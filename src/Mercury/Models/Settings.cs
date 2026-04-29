using System.Collections.ObjectModel;
using Mercury.Core.Models;
using Microsoft.VisualBasic;

namespace Mercury.Models;

public class PlayerSettings
{
    public int Volume { get; set; } = 60;

    public RepeatState RepeatState { get; set; } = RepeatState.RepeatSingle;

    public Track? LastTrack { get; set; } = null;

    public Playlist? LastPlaylist { get; set; } = null;
    
    public Collection<Track> Queue { get; set; } = new Collection<Track>();


    public static readonly PlayerSettings Default = new PlayerSettings();
}


public class DesignSettings
{
    public SystemTheme Theme { get; set; } = SystemTheme.UseSystemTheme;
    
    public bool CanLyricsAutoScroll { get; set; } = true;
    
    public static readonly DesignSettings Default = new DesignSettings();
}