using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core.Models;

namespace Mercury.Models;

public partial class PlayerSettings : ObservableObject
{
    public PlayerSettings()
    {
        Queue!.CollectionChanged += (_, _) => SettingsChanged?.Invoke(this);
    }
    
    [ObservableProperty]
    public partial int Volume { get; set; } = 60;
    
    [ObservableProperty]
    public partial RepeatState RepeatState { get; set; } = RepeatState.RepeatSingle;

    [ObservableProperty]
    public partial Track? LastTrack { get; set; } = null;

    [ObservableProperty]
    public partial Playlist? LastPlaylist { get; set; } = null;
    
    [ObservableProperty]
    public partial ObservableCollection<Track> Queue { get; set; } = [];
    
    public event Action<PlayerSettings>? SettingsChanged;
    
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        
        SettingsChanged?.Invoke(this);
    }


    public static readonly PlayerSettings Default = new PlayerSettings();

    internal JsonPlayerSettings AsJson()
        => new JsonPlayerSettings()
        {
            Volume = this.Volume,
            RepeatState = this.RepeatState,
            LastTrackId = this.LastTrack?.Id ?? string.Empty,
            LastPlaylistId = this.LastPlaylist?.Id ?? string.Empty,
            QueueIds = this.Queue.Select(t => t.Id).ToArray()
        };
}

internal class JsonPlayerSettings
{
    public required int Volume { get; init; }
    public required RepeatState RepeatState { get; init; }
    public required string LastTrackId { get; init; }
    public required string LastPlaylistId { get; init; }
    public string[] QueueIds { get; init; } = [];
}


public class DesignSettings
{
    public SystemTheme SystemTheme { get; set; } = SystemTheme.UseSystemTheme;
    public string UserThemeId { get; set; } = UiTheme.Default.Manifest.Id;
    
    public static readonly DesignSettings Default = new DesignSettings();
}