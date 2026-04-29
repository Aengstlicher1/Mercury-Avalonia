using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Mercury.Models;

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

        string playerJson = JsonSerializer.Serialize(PlayerSettings, options);
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
            PlayerSettings = JsonSerializer.Deserialize<PlayerSettings>(text) ?? PlayerSettings.Default;
        }

        var designTarget = Path.Combine(targetFolder, "design.json");
        if (File.Exists(designTarget))
        {
            var text = File.ReadAllText(playerTarget);
            PlayerSettings = JsonSerializer.Deserialize<PlayerSettings>(text) ?? PlayerSettings.Default;
        }
    }
    
    
    public override void OnExit()
    {
        Save();
    }
}