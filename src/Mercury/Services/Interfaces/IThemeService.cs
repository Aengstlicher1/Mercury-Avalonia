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
    
    ObservableCollection<UserTheme> AvailableThemes { get; }
    UserTheme CurrentTheme { get; }
    
    bool InstallFromPackage(string packagePath);
    void ApplyTheme(string themeId);
    void ApplyTheme(UserTheme theme);
    void UninstallPackage(string themeId);
    
    event Action<UserTheme?>? ThemeChanged;
    event Action<UserTheme?>? ThemeInstalled;
    event Action<UserTheme?>? ThemeUninstalled;
}