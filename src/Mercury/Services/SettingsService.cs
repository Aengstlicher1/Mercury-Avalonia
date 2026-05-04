using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Models;

namespace Mercury.Services;

public class SettingsService : ServiceBase, ISettingsService
{
    public PlayerSettings PlayerSettings { get; private set; } = PlayerSettings.Default;
    
    public DesignSettings DesignSettings { get; private set; } = DesignSettings.Default;

    
    public SettingsService()
    {
        _ = InitializeAsync();
    }
    
    public async Task InitializeAsync()
    {
        await Load();
    }
    
    public async Task Save()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string targetFolder = Path.Combine(appData, "Mercury");
        
        if (!Directory.Exists(targetFolder))
            Directory.CreateDirectory(targetFolder);
        
        var options = new JsonSerializerOptions { WriteIndented = true };

        string playerJson = JsonSerializer.Serialize(PlayerSettings.AsJson(), options);
        await File.WriteAllTextAsync(Path.Combine(targetFolder, "player.json"), playerJson);

        string designJson = JsonSerializer.Serialize(DesignSettings, options);
        await File.WriteAllTextAsync(Path.Combine(targetFolder, "design.json"), designJson);
    }

    public async Task Load()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string targetFolder = Path.Combine(appData, "Mercury");
        
        if (!Directory.Exists(targetFolder))
            return;

        var playerTarget = Path.Combine(targetFolder, "player.json");
        if (File.Exists(playerTarget))
        {
            var text = await File.ReadAllTextAsync(playerTarget);
            var json = JsonSerializer.Deserialize<JsonPlayerSettings>(text);

            if (json != null)
            {
                PlayerSettings.Volume = json.Volume;
                PlayerSettings.RepeatState = json.RepeatState;

                if (!string.IsNullOrWhiteSpace(json.LastTrackId))
                {
                    var lastTrack = await YoutubeMusic.Browse.GetAsync(json.LastTrackId);
                    PlayerSettings.LastTrack = lastTrack as Track;
                }

                if (!string.IsNullOrWhiteSpace(json.LastPlaylistId))
                {
                    var lastPlaylist = await YoutubeMusic.Browse.GetAsync(json.LastPlaylistId);
                    PlayerSettings.LastPlaylist = lastPlaylist as Playlist;
                }

                if (json.QueueIds.Any())
                {
                    Collection<Track> queue = [];
                    foreach (var id in json.QueueIds)
                    {
                        var queueItem = await YoutubeMusic.Browse.GetAsync(id);
                        if (queueItem is Track track) queue.Add(track);
                    }
                
                    PlayerSettings.Queue = new (queue);
                }
            }
        }

        var designTarget = Path.Combine(targetFolder, "design.json");
        if (File.Exists(designTarget))
        {
            var text = await File.ReadAllTextAsync(designTarget);
            DesignSettings = JsonSerializer.Deserialize<DesignSettings>(text) ?? DesignSettings.Default;
        }
    }
    
    
    public override void OnExit()
    {
        
    }
}