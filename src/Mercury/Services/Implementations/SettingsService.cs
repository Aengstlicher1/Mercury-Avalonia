using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Threading;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Services.Interfaces;

namespace Mercury.Services.Implementations;

public class SettingsService : ServiceBase, ISettingsService
{
    public PlayerSettings PlayerSettings { get; } = PlayerSettings.Default;
    
    public DesignSettings DesignSettings { get; } = DesignSettings.Default;

    
    public SettingsService()
    {
        Load();
    }
    
    public void Save()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string targetFolder = Path.Combine(appData, "Mercury");
        Directory.CreateDirectory(targetFolder); // no need to check first

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        try
        {
            string playerJson = JsonSerializer.Serialize<JsonPlayerSettings>(PlayerSettings.AsJson(), options);
            File.WriteAllText(Path.Combine(targetFolder, "player.json"), playerJson);

            string designJson = JsonSerializer.Serialize(DesignSettings, options);
            File.WriteAllText(Path.Combine(targetFolder, "design.json"), designJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Settings] Save failed: {ex}");
            throw;
        }
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

                _ = Task.Run(async () =>
                {
                    try { await LoadAsync(json); }
                    catch (Exception ex) { Console.WriteLine(ex); }
                });
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

    private async Task LoadAsync(JsonPlayerSettings json)
    {
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
    
    
    public override void OnExit()
    {
        
    }
}