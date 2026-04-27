using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Mercury.Core.Models;
using Mercury.Models;

namespace Mercury.Services;

public interface IPlayerService
{
    int Volume { get; set; }
    float Position { get; set; }
    
    RepeatState RepeatState  { get; set; }
    Track? CurrentTrack { get; set; }
    Collection<Track> CurrentQueue { get; set; }
    Playlist? CurrentPlaylist { get; set; }
    
    Task SetTrack(Track track, bool autoPlay = true);
    // Task SetPlaylist(Playlist playlist, bool autoPlay = true);

    void StartPlayblack();
    void PausePlayblack();
    void StopPlayblack();
    
    Task SkipForward(bool autoPlay = true);
    Task SkipBack(bool autoPlay = true);
    Task Skip(int relativeIndex, bool autoPlay = true);

    event Action<float>? PositionChanged;
    event Action<RepeatState>? RepeatStateChanged;
    event Action<bool>? PlayingChanged;
    event Action<int>? VolumeChanged;
    event Action<Track>? CurrentTrackChanged;
    event Action<Playlist>? CurrentPlaylistChanged;
    
    void Dispose();
}