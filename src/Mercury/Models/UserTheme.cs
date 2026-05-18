using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using Mercury.Services.Implementations;

namespace Mercury.Models;

public class UserTheme(ThemeManifest manifest, string folderName)
{
    public ThemeManifest Manifest { get; } = manifest;
    public string ThemeFolderName { get; } = folderName;

    public bool SupportsLight => Manifest.SupportedThemes.Any(x => x.ToLower() == "light");
    public bool SupportsDark => Manifest.SupportedThemes.Any(x => x.ToLower() == "dark");
    
    public string FullLightPath => Path.Combine(ThemeService.ThemesDir, ThemeFolderName, Manifest.LightPath);
    public string FullDarkPath => Path.Combine(ThemeService.ThemesDir, ThemeFolderName, Manifest.DarkPath);
    
    /* Cache the dirs */
    private string[]? _fullStylesPaths;
    private string[]? _fullResourcesPaths;

    public string[] FullStylesPaths    => _fullStylesPaths    ??= GetPaths("Styles");
    public string[] FullResourcesPaths => _fullResourcesPaths ??= GetPaths("Resources");

    private string[] GetPaths(string folderName)
    {
        var targetDir = Path.Combine(ThemeService.ThemesDir, ThemeFolderName, folderName);
        return Directory.GetFiles(targetDir, "*.axaml"); // already returns string[], no need for ToArray()
    }


    public static readonly UserTheme Default = new(
        new ThemeManifest
        {
            Id = "com.mercury.default",
            Name = "Default",
            Author = "Mercury",
            Version = new Version(1, 0, 0),
            SupportedThemes = 
            [
                "Light", 
                "Dark"
            ],
            LightPath = "Light.axaml",
            DarkPath = "Dark.axaml",
            PreviewPath = "Assets/preview.png"
        },
        folderName: "Default");
}