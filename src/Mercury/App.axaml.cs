using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using IconPacks.Avalonia.MaterialDesign;
using Mercury.Controls;
using Mercury.Services;
using Mercury.Services.Implementations;
using Mercury.Services.Interfaces;
using Mercury.ViewModels;
using Mercury.Views;
using Microsoft.Extensions.DependencyInjection;
using EntityViewerViewModel = Mercury.ViewModels.EntityViewerViewModel;
using LyricService = Mercury.Services.Implementations.LyricService;
using LyricsViewerViewModel = Mercury.ViewModels.LyricsViewerViewModel;
using NavigationService = Mercury.Services.Implementations.NavigationService;
using PlayerService = Mercury.Services.Implementations.PlayerService;
using QueueViewerViewModel = Mercury.ViewModels.QueueViewerViewModel;
using SearchService = Mercury.Services.Implementations.SearchService;


namespace Mercury;

public class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<INavigationService>());
        services.AddSingleton<ISearchService, SearchService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<ISearchService>());
        services.AddSingleton<IPlayerService, PlayerService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<IPlayerService>());
        services.AddSingleton<ILyricService, LyricService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<ILyricService>());
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<ISettingsService>());
        services.AddSingleton<IDiscordService, DiscordService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<IDiscordService>());
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<IThemeService>());

        
        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<HomePageViewModel>();
        services.AddTransient<ExplorePageViewModel>();
        services.AddTransient<SearchPageViewModel>();
        services.AddTransient<PlayingPageViewModel>();
        services.AddTransient<LyricsViewerViewModel>();
        services.AddTransient<QueueViewerViewModel>();
        services.AddTransient<EntityViewerViewModel>();
        services.AddTransient<PlaylistViewerViewModel>();

        // Register Views
        services.AddTransient<MainWindow>();
        services.AddTransient<HomePage>();
        services.AddTransient<ExplorePage>();
        services.AddTransient<SearchPage>();
        services.AddTransient<PlayingPage>();
        services.AddTransient<EntityViewer>();
        services.AddTransient<PlaylistViewer>();

        Services = services.BuildServiceProvider();
        
        
        /* Initialization */
        var discordService = Services.GetRequiredService<IDiscordService>();
        discordService.Initialize();
        var themeService = Services.GetRequiredService<IThemeService>();
        themeService.Initialize();
        
        var nav = Services.GetRequiredService<INavigationService>();
        nav.Register<HomePage, HomePageViewModel>("Home", PackIconMaterialDesignKind.HomeRound, isTab: true);
        nav.Register<ExplorePage, ExplorePageViewModel>("Explore", PackIconMaterialDesignKind.ExploreRound, isTab: true);
        nav.Register<SearchPage, SearchPageViewModel>("Search", PackIconMaterialDesignKind.SearchRound, isTab: false);
        nav.Register<PlayingPage, PlayingPageViewModel>("Playing", PackIconMaterialDesignKind.PlayArrowRound, isTab: false);
        
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
            
            /* Let the Services shutdown correctly and save the Settings */
            desktop.ShutdownRequested += (_, _) =>
            {
                foreach (var svc in Services.GetServices<IServiceBase>())
                    svc.OnExit();

                Services.GetService<ISettingsService>()?.Save();
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}