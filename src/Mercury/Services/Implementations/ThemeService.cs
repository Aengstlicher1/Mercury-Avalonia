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
    
    public async Task InitializeAsync()
    {
        Console.WriteLine("Initializing theme...");

        Directory.CreateDirectory(ThemesDir);
        
        // Discover all installed themes
        var found = new List<UiTheme>();
        foreach (var dir in Directory.EnumerateDirectories(ThemesDir))
        {
            var manifestPath = Path.Combine(dir, "theme.json");
            if (!File.Exists(manifestPath)) continue;
        
            try
            {
                var json = await File.ReadAllTextAsync(manifestPath);
                var manifest = JsonSerializer.Deserialize<ThemeManifest>(json);
                if (manifest is null || string.IsNullOrWhiteSpace(manifest.Id)) continue;
                
                found.Add(new UiTheme(manifest, Path.GetFileName(dir)));
            }
            catch
            {
                // Skip broken themes — don't let one bad folder kill startup.
            }
        }
        
        // AvailableThemes = new ObservableCollection<UiTheme>(found);
        Console.WriteLine("Initialized theme!");
        
        
        // Restore last theme, or fall back to Default
        var lastId = _settingsService.DesignSettings.UserThemeId;
        var match  = found.FirstOrDefault(t => t.Manifest.Id == lastId);
        
        if (match is not null)
            await ApplyThemeAsync(match.Manifest.Id);
        else
            await ResetToDefaultThemeAsync();
    }

    
    public async Task InstallFromPackageAsync(string packagePath)
    {
        Console.WriteLine("Installing theme...");
        
        if (!File.Exists(packagePath)) return;
        if (!packagePath.EndsWith(".mercuryTheme", StringComparison.OrdinalIgnoreCase) &&
            !packagePath.EndsWith(".zip",          StringComparison.OrdinalIgnoreCase))
            return;

        Directory.CreateDirectory(ThemesDir);
        
        var tempPath = Path.Combine(Path.GetTempPath(), "Mercury_ThemeInstall_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempPath);

        try
        {
            await ZipFile.ExtractToDirectoryAsync(packagePath, tempPath, overwriteFiles: true);

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
                manifest = JsonSerializer.Deserialize<ThemeManifest>(await File.ReadAllTextAsync(manifestPath));
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

            // 5. Replace any existing install of this theme.
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
            if (tempPath is not null && Directory.Exists(tempPath))
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


    public async Task ApplyThemeAsync(UiTheme theme)
    {
        if (AvailableThemes.Any(x => x.Manifest.Id == theme.Manifest.Id))
        {
            await ApplyThemeAsync(theme.Manifest.Id);
        }
        else
        {
            AvailableThemes.Add(theme);
            ThemeInstalled?.Invoke(theme);
            await ApplyThemeAsync(theme.Manifest.Id);
        }
    }
    
    public async Task ApplyThemeAsync(string themeId)
    {
        Console.WriteLine("Applying theme...");
        
        if (themeId == UiTheme.Default.Manifest.Id) { await ResetToDefaultThemeAsync(); return; }

        var theme = AvailableThemes.FirstOrDefault(x => x.Manifest.Id == themeId);
        if (theme is null) return;

        // Phase 1 — file I/O (will jump off UI thread on the awaits, fine)
        var lightDict = theme.SupportsLight ? await LoadDictionaryAsync(theme.FullLightPath) : null;
        var darkDict  = theme.SupportsDark  ? await LoadDictionaryAsync(theme.FullDarkPath)  : null;

        var styleContainers    = new List<UserThemeStyles>();
        foreach (var p in theme.FullStylesPaths)
            styleContainers.Add(await LoadStylesAsync(p));

        var resourceContainers = new List<UserThemeResources>();
        foreach (var p in theme.FullResourcesPaths)
            resourceContainers.Add(await LoadResourcesAsync(p));

        // Phase 2 — mutation. Jump back to UI thread if we're not on it.
        if (!Dispatcher.UIThread.CheckAccess())
            await Dispatcher.UIThread.InvokeAsync(ApplyToApp);
        else
            ApplyToApp();

        void ApplyToApp()
        {
            var app = Application.Current!;
            var dicts = app.Resources.ThemeDictionaries;

            if (lightDict is not null) dicts[ThemeVariant.Light] = lightDict;
            if (darkDict  is not null) dicts[ThemeVariant.Dark]  = darkDict;

            for (int i = app.Styles.Count - 1; i >= 0; i--)
                if (app.Styles[i] is UserThemeStyles) app.Styles.RemoveAt(i);

            for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
                if (app.Resources.MergedDictionaries[i] is UserThemeResources)
                    app.Resources.MergedDictionaries.RemoveAt(i);

            foreach (var s in styleContainers)    app.Styles.Add(s);
            foreach (var r in resourceContainers) app.Resources.MergedDictionaries.Add(r);

            CurrentTheme = theme;
            _settingsService.DesignSettings.UserThemeId = theme.Manifest.Id;
            
            Console.WriteLine("Theme applied!");
        }
    }

    private async Task<ResourceDictionary> LoadDictionaryAsync(string colorPath)
    {
        var xaml = await File.ReadAllTextAsync(colorPath);
        var obj = AvaloniaRuntimeXamlLoader.Load(xaml);
        return (ResourceDictionary)obj;
    }
    private async Task<UserThemeStyles> LoadStylesAsync(string stylesPath)
    {
        var xaml   = await File.ReadAllTextAsync(stylesPath);
        var loaded = (Styles)AvaloniaRuntimeXamlLoader.Load(xaml);

        var marker = new UserThemeStyles();
        foreach (var s in loaded) marker.Add(s);
        return marker;
    }
    private async Task<UserThemeResources> LoadResourcesAsync(string resourcesPath)
    {
        var xaml   = await File.ReadAllTextAsync(resourcesPath);
        var loaded = (ResourceDictionary)AvaloniaRuntimeXamlLoader.Load(xaml);

        var marker = new UserThemeResources();
        marker.MergedDictionaries.Add(loaded);
        return marker;
    }

    /// <summary>
    /// HAS to be called from the UiThread!
    /// (You can use <c>Dispatcher.UiThread.Invoke()</c>)
    /// </summary>
    public Task ResetToDefaultThemeAsync()
    {
        Console.WriteLine("Resetting theme...");
        
        if (!Dispatcher.UIThread.CheckAccess())
            return Dispatcher.UIThread.InvokeAsync(Reset).GetTask();

        Reset();
        return Task.CompletedTask;
        
        void Reset()
        {
            var app = Application.Current!;

            // Remove user-injected styles
            for (int i = app.Styles.Count - 1; i >= 0; i--)
                if (app.Styles[i] is UserThemeStyles)
                    app.Styles.RemoveAt(i);

            // Remove user-injected resource dictionaries
            for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
                if (app.Resources.MergedDictionaries[i] is UserThemeResources)
                    app.Resources.MergedDictionaries.RemoveAt(i);

            // Restore the original Light/Dark dictionaries from App.axaml.
            // The cleanest way: keep references to the originals captured at startup.
            // TODO: Unshittify this and copy default on init

            CurrentTheme = UiTheme.Default;
            _settingsService.DesignSettings.UserThemeId = UiTheme.Default.Manifest.Id;
            
            Console.WriteLine("Theme reset!");
        }
    }

    
    public async Task UninstallPackageAsync(string themeId)
    {
        var theme = AvailableThemes.FirstOrDefault(x => x.Manifest.Id == themeId);
        if (theme is null) return;

        if (CurrentTheme.Manifest.Id == themeId)
            await ResetToDefaultThemeAsync();

        var folder = Path.Combine(ThemesDir, theme.ThemeFolderName);
        if (Directory.Exists(folder))
            Directory.Delete(folder, recursive: true);

        AvailableThemes.Remove(theme);
        ThemeUninstalled?.Invoke(theme);
    }


    partial void OnCurrentThemeChanged(UiTheme? value)
    {
        ThemeChanged?.Invoke(value);
    }


    public override void OnExit()
    {
        
    }
}