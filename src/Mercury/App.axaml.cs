using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Mercury.Services;
using Mercury.ViewModels;
using Mercury.Views;
using Microsoft.Extensions.DependencyInjection;


namespace Mercury;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISearchService, SearchService>();
        services.AddSingleton<IPlayerService, PlayerService>();
        services.AddSingleton<ILyricService, LyricService>();
        
        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<HomePageViewModel>();
        services.AddTransient<ExplorePageViewModel>();
        services.AddTransient<SearchPageViewModel>();
        services.AddTransient<PlayingPageViewModel>();
        services.AddTransient<LyricsViewerViewModel>();

        // Register Views
        services.AddTransient<MainWindow>();
        services.AddTransient<HomePage>();
        services.AddTransient<ExplorePage>();
        services.AddTransient<SearchPage>();
        services.AddTransient<PlayingPage>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}