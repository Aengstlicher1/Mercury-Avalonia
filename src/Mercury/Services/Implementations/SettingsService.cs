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
using Mercury.Services.Interfaces;

namespace Mercury.Services;

public class SettingsService : ServiceBase, ISettingsService
{
    public PlayerSettings PlayerSettings { get; private set; } = PlayerSettings.Default;
    
    public DesignSettings DesignSettings { get; private set; } = DesignSettings.Default;

    
    public SettingsService()
    {
        Load();
    }
    
    public void Save()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string targetFolder = Path.Combine(appData, "Mercury");
        
        if (!Directory.Exists(targetFolder))
            Directory.CreateDirectory(targetFolder);
        
        var options = new JsonSerializerOptions { WriteIndented = true };

        string playerJson = JsonSerializer.Serialize(PlayerSettings.AsJson(), options);
        File.WriteAllText(Path.Combine(targetFolder, "player.json"), playerJson);

        string designJson = JsonSerializer.Serialize(DesignSettings, options);
        File.WriteAllText(Path.Combine(targetFolder, "design.json"), designJson);
    }

    public void Load()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string targetFolder = Path.Combine(appData, "Mercury");
        
        if (!Directory.Exists(targetFolder))
            return;

        var playerTarget = Path.Combine(targetFolder, "player.json");
        if (File.Exists(playerTarget))
        {
            var text = File.ReadAllText(playerTarget);
            var json = JsonSerializer.Deserialize<JsonPlayerSettings>(text);

            if (json != null)
            {
                PlayerSettings.Volume = json.Volume;
                PlayerSettings.RepeatState = json.RepeatState;

                if (!string.IsNullOrWhiteSpace(json.LastTrackId))
                {
                    var lastTrack = YoutubeMusic.Browse.GetAsync(json.LastTrackId).GetAwaiter().GetResult();
                    PlayerSettings.LastTrack = lastTrack as Track;
                }

                if (!string.IsNullOrWhiteSpace(json.LastPlaylistId))
                {
                    var lastPlaylist = YoutubeMusic.Browse.GetAsync(json.LastPlaylistId).GetAwaiter().GetResult();
                    PlayerSettings.LastPlaylist = lastPlaylist as Playlist;
                }

                if (json.QueueIds.Any())
                {
                    Collection<Track> queue = [];
                    foreach (var id in json.QueueIds)
                    {
                        var queueItem = YoutubeMusic.Browse.GetAsync(id).GetAwaiter().GetResult();
                        if (queueItem is Track track) queue.Add(track);
                    }
                
                    PlayerSettings.Queue = new (queue);
                }
            }
        }

        var designTarget = Path.Combine(targetFolder, "design.json");
        if (File.Exists(designTarget))
        {
            var text = File.ReadAllText(designTarget);
            var json = JsonSerializer.Deserialize<DesignSettings>(text);

            if (json != null)
            {
                DesignSettings.SystemTheme = json.SystemTheme;
                DesignSettings.UserThemeId = json.UserThemeId;
            }
        }
    }
    
    
    public override void OnExit()
    {
        
    }
}