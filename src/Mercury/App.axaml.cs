using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using IconPacks.Avalonia.MaterialDesign;
using Mercury.Controls;
using Mercury.Services;
using Mercury.ViewModels;
using Mercury.Views;
using Microsoft.Extensions.DependencyInjection;
using EntityViewerViewModel = Mercury.ViewModels.EntityViewerViewModel;
using LyricsViewerViewModel = Mercury.ViewModels.LyricsViewerViewModel;
using QueueViewerViewModel = Mercury.ViewModels.QueueViewerViewModel;


namespace Mercury;

public partial class App : Application
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
        
        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<MainWindowViewModel>());
        services.AddTransient<HomePageViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<HomePageViewModel>());
        services.AddTransient<ExplorePageViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<ExplorePageViewModel>());
        services.AddTransient<SearchPageViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<SearchPageViewModel>());
        services.AddTransient<PlayingPageViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<PlayingPageViewModel>());
        services.AddTransient<LyricsViewerViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<LyricsViewerViewModel>());
        services.AddTransient<QueueViewerViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<QueueViewerViewModel>());
        services.AddTransient<EntityViewerViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<EntityViewerViewModel>());
        services.AddTransient<PlaylistViewerViewModel>();
        services.AddTransient<ViewModelBase>(sp => sp.GetRequiredService<PlaylistViewerViewModel>());

        // Register Views
        services.AddTransient<MainWindow>();
        services.AddTransient<HomePage>();
        services.AddTransient<ExplorePage>();
        services.AddTransient<SearchPage>();
        services.AddTransient<PlayingPage>();
        services.AddTransient<EntityViewer>();
        services.AddTransient<PlaylistViewer>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        }

        /* Initialization */
        var nav = Services.GetRequiredService<INavigationService>();
        nav.Register<HomePage, HomePageViewModel>("Home", PackIconMaterialDesignKind.HomeRound, isTab: true);
        nav.Register<ExplorePage, ExplorePageViewModel>("Explore", PackIconMaterialDesignKind.ExploreRound, isTab: true);
        nav.Register<SearchPage, SearchPageViewModel>("Search", PackIconMaterialDesignKind.SearchRound, isTab: false);
        nav.Register<PlayingPage, PlayingPageViewModel>("Playing", PackIconMaterialDesignKind.PlayArrowRound, isTab: false);
        
        var discordService = Services.GetRequiredService<IDiscordService>();
        discordService.Initialize();
        
        base.OnFrameworkInitializationCompleted();
    }
}