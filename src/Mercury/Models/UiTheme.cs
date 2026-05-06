using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using Mercury.Services.Implementations;

namespace Mercury.Models;

public class UiTheme(ThemeManifest manifest, string folderName)
{
    public ThemeManifest Manifest { get; } = manifest;
    public string ThemeFolderName { get; } = folderName;

    public bool SupportsLight => Manifest.SupportedThemes.Any(x => x.ToLower() == "light");
    public bool SupportsDark => Manifest.SupportedThemes.Any(x => x.ToLower() == "dark");
    
    public string FullLightPath => Path.Combine(ThemeService.ThemesDir, ThemeFolderName, Manifest.LightPath);
    public string FullDarkPath => Path.Combine(ThemeService.ThemesDir, ThemeFolderName, Manifest.DarkPath);
    public string[] FullStylesPaths => Manifest.StylesPaths.Select(x => Path.Combine(ThemeService.ThemesDir, ThemeFolderName, "Styles", x)).ToArray();
    public string[] FullResourcesPaths => Manifest.ResourcesPaths.Select(x => Path.Combine(ThemeService.ThemesDir, ThemeFolderName, "Resources", x)).ToArray();


    public static readonly UiTheme Default = new(
        new ThemeManifest
        {
            Id = "com.mercury.template",
            Name = "Default",
            Author = "Mercury",
            Version = new Version(1, 0, 0),
            SupportedThemes = new[] { "Light", "Dark" }
        },
        folderName: string.Empty);
}