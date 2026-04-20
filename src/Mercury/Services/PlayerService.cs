using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using Mercury.Core;
using Mercury.Core.Models;
using Media = LibVLCSharp.Shared.Media;

namespace Mercury.Services;

public partial class PlayerService : ServiceBase, IPlayerService, IDisposable
{
    public event Action<float>? PositionChanged;
    public event Action<bool>? PlayingChanged;
    public event Action<int>? VolumeChanged;
    public event Action<Track>? CurrentTrackChanged;
    public event Action<Playlist>? CurrentPlaylistChanged;

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
    private Track? _currentTrack;
    
    [ObservableProperty]
    private Collection<Track> _currentQueue = new ();

    [ObservableProperty]
    private Playlist? _currentPlaylist;

    
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
    }

    
    public async Task SetTrack(Track track, bool autoPlay)
    {
        try
        {
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
    
    public async Task Skip(int relativeIndex, bool autoPlay = true)
    {
        if (CurrentTrack != null && CurrentQueue.FirstOrDefault(t => t.Id == CurrentTrack.Id) != null)
        {
            int currentIndex = CurrentQueue.IndexOf(CurrentQueue.First(t => t.Id == CurrentTrack.Id));
            if (currentIndex == -1) return;
            
            int targetIndex = (currentIndex + relativeIndex) % CurrentQueue.Count;
            if (targetIndex < 0) targetIndex += CurrentQueue.Count;
            
            var target = CurrentQueue.ElementAt(targetIndex);
            
            await SetTrack(target, autoPlay);
        }
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
}
