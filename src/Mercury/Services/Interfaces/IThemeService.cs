using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Mercury.Models;

namespace Mercury.Services.Interfaces;

public interface IThemeService : IServiceBase
{
    Task InitializeAsync();
    
    ObservableCollection<UiTheme> AvailableThemes { get; }
    UiTheme? CurrentTheme { get; }
    
    Task InstallFromPackageAsync(string packagePath);
    Task ApplyThemeAsync(string themeId);
    Task ApplyThemeAsync(UiTheme theme);
    Task ResetToDefaultThemeAsync();
    Task UninstallPackageAsync(string themeId);
    
    event Action<UiTheme?> ThemeChanged;
    event Action<UiTheme?> ThemeInstalled;
    event Action<UiTheme?> ThemeUninstalled;
}