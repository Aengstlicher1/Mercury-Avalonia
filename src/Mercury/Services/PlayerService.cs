using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Models;
using Microsoft.Extensions.DependencyInjection;
using Media = LibVLCSharp.Shared.Media;

namespace Mercury.Services;

public partial class PlayerService : ServiceBase, IPlayerService, IDisposable
{
    public event Action<float>? PositionChanged;
    public event Action<bool>? PlayingChanged;
    public event Action<int>? VolumeChanged;
    public event Action<Track>? CurrentTrackChanged;
    public event Action<Playlist>? CurrentPlaylistChanged;
    public event Action<RepeatState>? RepeatStateChanged;

    private readonly LibVLC _libVlc;
    private readonly MediaPlayer _mediaPlayer;
    private Media? _currentMedia;


    /// <summary>
    /// Volume: 0 - 100
    /// </summary>
    public int Volume
    {
        get;
        set
        {
            value = Math.Clamp(value, 0, 100);
            if (field == value) return;

            field = value;

            if (_mediaPlayer.Media is not null)
                _mediaPlayer.Volume = field;

            VolumeChanged?.Invoke(field); // notify UI immediately
        }
    } = 50;


    /// <summary>
    /// Position: 0.0f - 1.0f
    /// </summary>
    public float Position
    {
        get => _mediaPlayer.Position;
        set
        {
            if (Math.Abs(_mediaPlayer.Position - value) < 0.005f) return;
            
            _mediaPlayer.Position = value;
            PositionChanged?.Invoke(value);
        }
    }

    [ObservableProperty]
    private RepeatState _repeatState = RepeatState.RepeatSingle;
    
    [ObservableProperty]
    private Track? _currentTrack;
    
    [ObservableProperty]
    private Playlist? _currentPlaylist;
    
    [ObservableProperty]
    private Collection<Track> _currentQueue = new ();
    
    private Collection<Track> ShuffledTracks { get; set; } = new ();
    
    public PlayerService()
    {
        // "--no-video" ensures no video decoding/rendering — audio only
        _libVlc = new LibVLC("--no-video");
        _mediaPlayer = new MediaPlayer(_libVlc);
        
        _mediaPlayer.PositionChanged += (_, e) =>
        {
            PositionChanged?.Invoke(e.Position);
        };

        _mediaPlayer.Playing += (_, _) =>
        {
            Console.WriteLine("VLC: Playback started");
            PlayingChanged?.Invoke(true);
        };

        _mediaPlayer.Paused += (_, _) =>
        {
            Console.WriteLine("VLC: Playback paused");
            PlayingChanged?.Invoke(false);
        };

        _mediaPlayer.Stopped += MediaStopped;

        LoadSettings();
    }


    private void LoadSettings()
    {
        var ss = App.Services.GetRequiredService<ISettingsService>();

        CurrentTrack = ss.PlayerSettings.LastTrack;
        CurrentPlaylist = ss.PlayerSettings.LastPlaylist;
        CurrentQueue = ss.PlayerSettings.Queue;
        
        Volume = ss.PlayerSettings.Volume;
        RepeatState = ss.PlayerSettings.RepeatState;
    }
    
    private void MediaStopped(object? sender, EventArgs e)
    {
        if (RepeatState is not RepeatState.NoRepeat)
        {
            if (RepeatState is RepeatState.RepeatSingle && CurrentTrack is not null)
            {
                _ = BaseSetTrack(CurrentTrack, true, false);
            }
            else if (RepeatState is RepeatState.RepeatAll)
            {
                _ = BaseSkip(+1, true, false);
            }
            else if (RepeatState is RepeatState.Shuffle)
            {
                var track = GetRandomTrack();
                _ = BaseSetTrack(track, true, false);
            }
        }
    }

    private Track GetRandomTrack()
    {
        if (!CurrentQueue.Any()) return CurrentTrack!;
        
        if (CurrentTrack is not null && !ShuffledTracks.Contains(CurrentTrack))
            ShuffledTracks.Add(CurrentTrack);
        
        var tracks = CurrentQueue.Where(t => !ShuffledTracks.Contains(t)).ToArray();
        if (tracks.Any())
        {
            var rndTrack = tracks.Shuffle().First();
            ShuffledTracks.Add(rndTrack);
            return rndTrack;
        }
        else
        {
            ShuffledTracks.Clear();
                    
            tracks = CurrentQueue.Where(t => !ShuffledTracks.Contains(t)).ToArray();
                    
            var rndTrack = tracks.Shuffle().First();
            ShuffledTracks.Add(rndTrack);
            return rndTrack;
        }
    }

    public async Task SetTrack(Track track, bool autoPlay, CancellationToken cToken = default)
    {
        CurrentQueue.Clear();
        CurrentPlaylist = null;
        await BaseSetTrack(track, autoPlay, cToken: cToken);
    }
    private async Task BaseSetTrack(Track track, bool autoPlay, bool preStop = true, CancellationToken cToken = default)
    {
        try
        {
            if (preStop)
                _mediaPlayer.Stop();
            _currentMedia?.Dispose();

            var streamData = await YoutubeMusic.Player.GetStreamDataAsync(track.Id, cToken);
            if (streamData is null) return;

            var stream = streamData.Streams
                .Where(s => s is AudioStreamInfo)
                .MaxBy(x => x.Bitrate)!;

            _currentMedia = new Media(_libVlc, new Uri(stream.Url));
            _mediaPlayer.Media = _currentMedia;

            CurrentTrack = track;
            CurrentTrack.Duration = streamData.Duration >= TimeSpan.FromHours(1) 
                ? streamData.Duration.ToString(@"h\:mm\:ss") 
                : streamData.Duration.ToString(@"m\:ss");

            if (autoPlay)
            {
                StartPlayblack();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SetTrack error: {ex}");
        }
    }


    public async Task SetPlaylist(Playlist playlist, bool autoPlay, CancellationToken cToken = default)
        => await BaseSetPlaylist(playlist, autoPlay, cToken: cToken);

    private async Task BaseSetPlaylist(Playlist playlist, bool autoPlay, bool preStop = true, CancellationToken cToken = default)
    {
        var mediaInfo = await YoutubeMusic.Browse.GetInfoAsync(playlist, cToken);

        if (mediaInfo is PlaylistInfo playlistInfo && playlistInfo.TracksCount > 0)
        {
            CurrentQueue.Clear();
            foreach (var track in playlistInfo.Tracks)
            {
                CurrentQueue.Add(track);
            }
            
            await BaseSetTrack(playlistInfo.Tracks.First(), autoPlay, preStop, cToken);
            
            CurrentPlaylist = playlist;
        }
    }
    
    public void StartPlayblack()
    {
        _mediaPlayer.Play();
        _mediaPlayer.Volume = Volume;
    }
    
    public void PausePlayblack()
    {
        _mediaPlayer.Pause();
    }
    
    public void StopPlayblack()
    {
        _mediaPlayer.Stop();
    }

    public async Task SkipForward(bool autoPlay = true)
        => await Skip(+1, autoPlay);
    
    public async Task SkipBack(bool autoPlay = true)
        => await Skip(-1, autoPlay);
    
    public async Task Skip(int relativeIndex, bool autoPlay = true) => await BaseSkip(relativeIndex, autoPlay);
    private async Task BaseSkip(int relativeIndex, bool autoPlay = true, bool preStop = true)
    {
        if (CurrentTrack is null || CurrentQueue.Count == 0) return;
        
        var match = CurrentQueue.FirstOrDefault(t => t.Id == CurrentTrack.Id);
        Track target;
        
        if (match is not null)
        {
            int currentIndex = CurrentQueue.IndexOf(match);
            int targetIndex = (currentIndex + relativeIndex) % CurrentQueue.Count;
            if (targetIndex < 0) targetIndex += CurrentQueue.Count;

            target = RepeatState is RepeatState.Shuffle
                ? GetRandomTrack()
                : CurrentQueue.ElementAt(targetIndex);
        }
        else
        {
            target = relativeIndex > 0
                ? CurrentQueue.First()
                : CurrentQueue.Last();
        }
        
        await BaseSetTrack(target, autoPlay, preStop);
    } 
    
    
    public void Dispose()
    {
        _mediaPlayer.Stop();
        _currentMedia?.Dispose();
        _mediaPlayer.Dispose();
        _libVlc.Dispose();
    }

    partial void OnCurrentTrackChanged(Track? value)
    {
        CurrentTrackChanged?.Invoke(value!);
    }
    partial void OnCurrentPlaylistChanged(Playlist? value)
    {
        CurrentPlaylistChanged?.Invoke(value!);
    }
    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnCurrentQueueChanged(Collection<Track> value)
    {
        ShuffledTracks.Clear();
    }
    partial void OnRepeatStateChanged(RepeatState value)
    {
        RepeatStateChanged?.Invoke(value);
    }
    
    
    public override void OnExit()
    {
        Dispose();
        
        var ss = App.Services.GetRequiredService<ISettingsService>();

        ss.PlayerSettings.LastTrack = CurrentTrack;
        ss.PlayerSettings.LastPlaylist = CurrentPlaylist;
        ss.PlayerSettings.Queue = CurrentQueue;
        
        ss.PlayerSettings.Volume = Volume;
        ss.PlayerSettings.RepeatState = RepeatState;
    }
}
