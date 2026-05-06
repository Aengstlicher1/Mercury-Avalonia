using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Mercury.Models;

namespace Mercury.Services.Interfaces;

public interface IThemeService : IServiceBase
{
    void Initialize();
    
    ObservableCollection<UiTheme> AvailableThemes { get; }
    UiTheme? CurrentTheme { get; }
    
    void InstallFromPackage(string packagePath);
    void ApplyTheme(string themeId);
    void ApplyTheme(UiTheme theme);
    void UninstallPackage(string themeId);
    
    event Action<UiTheme?> ThemeChanged;
    event Action<UiTheme?> ThemeInstalled;
    event Action<UiTheme?> ThemeUninstalled;
}