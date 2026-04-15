using System;
using System.Threading.Tasks;
using Mercury.Core.Models;

namespace Mercury.Services;

public interface IPlayerService
{
    int Volume { get; set; }
    float Position { get; set; }
    
    Track? CurrentTrack { get; set; }
    Playlist? CurrentPlaylist { get; set; }
    
    Task SetTrack(Track track, bool autoPlay = true);
    // Task SetPlaylist(Playlist playlist, bool autoPlay = true);

    void StartPlayblack();
    void PausePlayblack();
    void StopPlayblack();

    // Task MoveTrack(bool forwards = true);

    event Action<float>? PositionChanged;
    event Action<int>? VolumeChanged;
    event Action<Track>? CurrentTrackChanged;
    event Action<Playlist>? CurrentPlaylistChanged;
    
    void Dispose();
}