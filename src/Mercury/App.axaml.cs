using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<INavigationService>());
        services.AddSingleton<ISearchService, SearchService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<ISearchService>());
        services.AddSingleton<IPlayerService, PlayerService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<IPlayerService>());
        services.AddSingleton<ILyricService, LyricService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<ILyricService>());
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IServiceBase>(sp => sp.GetRequiredService<ISettingsService>());
        
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