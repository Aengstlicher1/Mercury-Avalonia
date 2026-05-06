using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Helpers;
using Mercury.Models;
using Mercury.Services.Interfaces;

namespace Mercury.Services.Implementations;

public partial class ThemeService(ISettingsService settingsService) : ServiceBase, IThemeService
{
    private readonly ISettingsService _settingsService = settingsService;
    
    public static readonly string ThemesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mercury", "Themes");
    
    [ObservableProperty]
    public partial ObservableCollection<UiTheme> AvailableThemes { get; set; } = new ();
    
    [ObservableProperty]
    public partial UiTheme CurrentTheme { get; set; } = UiTheme.Default;
    
    public event Action<UiTheme?>? ThemeChanged;
    public event Action<UiTheme?>? ThemeInstalled;
    public event Action<UiTheme?>? ThemeUninstalled;
    
    public void Initialize()
    {
        Directory.CreateDirectory(ThemesDir);

        var defaultsSrc = Path.Combine(AppContext.BaseDirectory, "Themes");
        if (Directory.Exists(defaultsSrc))
            DirectoryHelper.CopyAll(defaultsSrc, ThemesDir);

        var found = new List<UiTheme>();
        foreach (var dir in Directory.EnumerateDirectories(ThemesDir))
        {
            var manifestPath = Path.Combine(dir, "theme.json");
            if (!File.Exists(manifestPath)) continue;
            try
            {
                var manifest = JsonSerializer.Deserialize<ThemeManifest>(File.ReadAllText(manifestPath));
                if (manifest is null || string.IsNullOrWhiteSpace(manifest.Id)) continue;
                found.Add(new UiTheme(manifest, Path.GetFileName(dir)));
            }
            catch { /* skip */ }
        }

        AvailableThemes = new ObservableCollection<UiTheme>(found);

        var lastId = _settingsService.DesignSettings.UserThemeId;
        var match  = found.FirstOrDefault(t => t.Manifest.Id == lastId);
        ApplyTheme(match?.Manifest.Id ?? UiTheme.Default.Manifest.Id);
    }
    
    public void InstallFromPackage(string packagePath)
    {
        Console.WriteLine("Installing theme...");
        
        if (!File.Exists(packagePath)) return;
        if (!packagePath.EndsWith(".mercuryTheme", StringComparison.OrdinalIgnoreCase) &&
            !packagePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            return;

        Directory.CreateDirectory(ThemesDir);
        
        var tempPath = Path.Combine(Path.GetTempPath(), "Mercury_ThemeInstall_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempPath);

        try
        {
            ZipFile.ExtractToDirectory(packagePath, tempPath, overwriteFiles: true);

            // Find theme.json, as it indicates the actual content of the Theme — root first, then any single subfolder, then a recursive search as last resort.
            var manifestPath = LocateManifest(tempPath);
            if (manifestPath is null)
            {
                // No theme.json anywhere — invalid package.
                return;
            }

            // The "theme root" is the directory containing theme.json.
            var themeRoot = Path.GetDirectoryName(manifestPath)!;

            // 3. Read & validate manifest.
            ThemeManifest? manifest;
            try
            {
                manifest = JsonSerializer.Deserialize<ThemeManifest>(File.ReadAllText(manifestPath));
            }
            catch (JsonException)
            {
                return;
            }

            if (manifest is null || string.IsNullOrWhiteSpace(manifest.Id))
                return;

            // 4. Pick a folder name. Prefer manifest id (sanitized) so re-installs land in the same place.
            var folderName = SanitizeFolderName(manifest.Id);
            var targetPath = Path.Combine(ThemesDir, folderName);

            // 5. Replace any existing installation of this theme.
            if (Directory.Exists(targetPath))
                Directory.Delete(targetPath, recursive: true);

            // 6. Move the theme root into ThemesDir. If themeRoot == stagingPath (manifest at root),
            //    a Move would also move the staging folder itself — handle both cases.
            if (PathsEqual(themeRoot, tempPath))
            {
                // Manifest was at the zip root — move the staging folder to its final name.
                Directory.Move(tempPath, targetPath);
                tempPath = null!; // suppress finally cleanup; it's been renamed
            }
            else
            {
                // Manifest was inside a subfolder — move just that subfolder.
                Directory.Move(themeRoot, targetPath);
            }

            // 7. Wire it up.
            var theme = new UiTheme(manifest, folderName);

            // De-dupe in the in-memory list.
            var existing = AvailableThemes.FirstOrDefault(t => t.Manifest.Id == manifest.Id);
            if (existing is not null) AvailableThemes.Remove(existing);

            AvailableThemes.Add(theme);
            ThemeInstalled?.Invoke(theme);
        }
        finally
        {
            // Clean up staging if it still exists (success path with subfolder, or any failure).
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, recursive: true); } catch { /* ignore */ }
            }
            
            Console.WriteLine("Theme installed successfully!");
        }
    }

    private static string? LocateManifest(string root)
    {
        // Root level
        var atRoot = Path.Combine(root, "theme.json");
        if (File.Exists(atRoot)) return atRoot;

        // Single top-level subfolder containing theme.json
        var subdirs = Directory.GetDirectories(root);
        if (subdirs.Length == 1)
        {
            var nested = Path.Combine(subdirs.First(), "theme.json");
            if (File.Exists(nested)) return nested;
        }

        // Recursive search, take the first match.
        var match = Directory
            .EnumerateFiles(root, "theme.json", SearchOption.AllDirectories)
            .OrderBy(p => p.Count(c => c == Path.DirectorySeparatorChar))
            .FirstOrDefault();

        return match;
    }

    private static string SanitizeFolderName(string raw)
    {
        var cleaned = string.Concat(raw.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
        return string.IsNullOrWhiteSpace(cleaned) ? "theme_" + Guid.NewGuid().ToString("N")[..8] : cleaned;
    }

    private static bool PathsEqual(string a, string b)
    {
        var na = Path.GetFullPath(a).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var nb = Path.GetFullPath(b).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var cmp = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return string.Equals(na, nb, cmp);
    }


    public void ApplyTheme(UiTheme theme)
    {
        if (AvailableThemes.Any(x => x.Manifest.Id == theme.Manifest.Id))
        {
            ApplyTheme(theme.Manifest.Id);
        }
        else
        {
            throw new ArgumentException($"Theme:({theme.Manifest.Id}) is not Available.");
        }
    }
    
    public void ApplyTheme(string themeId)
    {
        var theme = AvailableThemes.FirstOrDefault(x => x.Manifest.Id == themeId);
        if (theme is null) return;

        var lightDict = theme.SupportsLight ? Load<ResourceDictionary>(theme.FullLightPath) : null;
        var darkDict  = theme.SupportsDark  ? Load<ResourceDictionary>(theme.FullDarkPath)  : null;
        var styleContainers    = theme.FullStylesPaths.Select(LoadStyles).ToList();
        var resourceContainers = theme.FullResourcesPaths.Select(LoadResources).ToList();

        var app = Application.Current!;
        var dicts = app.Resources.ThemeDictionaries;
        if (lightDict is not null) dicts[ThemeVariant.Light] = lightDict;
        if (darkDict  is not null) dicts[ThemeVariant.Dark]  = darkDict;

        for (int i = app.Styles.Count - 1; i >= 0; i--)
            if (app.Styles[i] is UserThemeStyles) app.Styles.RemoveAt(i);
        for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
            if (app.Resources.MergedDictionaries[i] is UserThemeResources)
                app.Resources.MergedDictionaries.RemoveAt(i);

        foreach (var s in styleContainers) app.Styles.Add(s);
        foreach (var r in resourceContainers) app.Resources.MergedDictionaries.Add(r);

        CurrentTheme = theme;
        _settingsService.DesignSettings.UserThemeId = theme.Manifest.Id;
    }

    private static T Load<T>(string path) => (T)AvaloniaRuntimeXamlLoader.Load(File.ReadAllText(path));

    private static UserThemeStyles LoadStyles(string p)
    {
        var m = new UserThemeStyles(); 
        foreach (var s in Load<Styles>(p)) 
            m.Add(s); 
        return m;
    }

    private static UserThemeResources LoadResources(string p)
    {
        var m = new UserThemeResources(); 
        m.MergedDictionaries.Add(Load<ResourceDictionary>(p)); 
        return m;
    }
    
    
    public void UninstallPackage(string themeId)
    {
        var theme = AvailableThemes.FirstOrDefault(x => x.Manifest.Id == themeId);
        if (theme is null) return;

        if (CurrentTheme.Manifest.Id == themeId)
            ApplyTheme(UiTheme.Default);

        var folder = Path.Combine(ThemesDir, theme.ThemeFolderName);
        if (Directory.Exists(folder))
            Directory.Delete(folder, recursive: true);

        AvailableThemes.Remove(theme);
        ThemeUninstalled?.Invoke(theme);
    }

    partial void OnCurrentThemeChanged(UiTheme value)
    {
        ThemeChanged?.Invoke(value);
    }


    public override void OnExit()
    {
        
    }
}