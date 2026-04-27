using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Models;
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
        
        _mediaPlayer.PositionChanged += (s, e) =>
        {
            PositionChanged?.Invoke(e.Position);
        };

        _mediaPlayer.Playing += (s, e) =>
        {
            Console.WriteLine("VLC: Playback started");
            PlayingChanged?.Invoke(true);
        };

        _mediaPlayer.Paused += (s, e) =>
        {
            Console.WriteLine("VLC: Playback paused");
            PlayingChanged?.Invoke(false);
        };

        _mediaPlayer.Stopped += MediaStopped;
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
    
    public async Task SetTrack(Track track, bool autoPlay) => await BaseSetTrack(track, autoPlay);
    private async Task BaseSetTrack(Track track, bool autoPlay, bool preStop = true)
    {
        try
        {
            if (preStop)
                _mediaPlayer.Stop();
            _currentMedia?.Dispose();

            var streamData = await YoutubeMusic.Player.GetStreamDataAsync(track.Id);
            if (streamData is null) return;

            var stream = streamData.Streams
                .Where(s => s is AudioStreamInfo)
                .MaxBy(x => x.Bitrate)!;

            _currentMedia = new Media(_libVlc, new Uri(stream.Url));
            _mediaPlayer.Media = _currentMedia;

            CurrentTrack = track;

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
    partial void OnCurrentQueueChanged(Collection<Track> value)
    {
        ShuffledTracks.Clear();
    }
    partial void OnRepeatStateChanged(RepeatState value)
    {
        RepeatStateChanged?.Invoke(value);
    }
}
