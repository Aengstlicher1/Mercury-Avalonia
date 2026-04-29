using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconPacks.Avalonia.MaterialDesign;
using Mercury.Models;
using Mercury.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Services;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, PageDescriptor> _registry = new();
    private readonly ObservableCollection<PageDescriptor> _tabs = [];
    private readonly Stack<(Type VmType, Control View)> _backStack = new();
    private readonly Dictionary<Type, (Control View, object ViewModel)> _pageCache = new();

    private Control? _currentView;
    private TransitioningContentControl? _host;

    [ObservableProperty]
    private PageDescriptor? _currentDescriptor;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SetHost(TransitioningContentControl host) => _host = host;

    // ── Registration ─────────────────────────────────────────────
    public IReadOnlyList<PageDescriptor> Tabs => _tabs;

    public void Register<TView, TViewModel>(
        string title, PackIconMaterialDesignKind icon, bool isTab = false)
        where TView : Control
        where TViewModel : class
    {
        var descriptor = new PageDescriptor(title, icon, typeof(TView), typeof(TViewModel), isTab);
        _registry[typeof(TView)] = descriptor;

        if (isTab)
            _tabs.Add(descriptor);
    }

    private PageDescriptor AutoRegister(Type viewType)
    {
        // HomePage → HomePageViewModel
        var viewModelTypeName = viewType.FullName!.Replace(".Views.", ".ViewModels.") + "ViewModel";
        var viewModelType = viewType.Assembly.GetType(viewModelTypeName);

        if (viewModelType is null)
            throw new InvalidOperationException(
                $"Could not auto-resolve ViewModel for '{viewType.Name}'. " +
                $"Expected '{viewModelTypeName}'. Register it manually instead.");

        // Derive a title from the type name: "PlaylistPage" → "Playlist"
        var title = viewType.Name.EndsWith("Page")
            ? viewType.Name[..^4]
            : viewType.Name;

        var descriptor = new PageDescriptor(
            Title: title,
            Icon: PackIconMaterialDesignKind.PagesRound, // default icon
            ViewType: viewType,
            ViewModelType: viewModelType,
            IsTab: false // auto-registered pages are never tabs
        );

        _registry[viewType] = descriptor;
        return descriptor;
    }
    
    // ── Navigation ───────────────────────────────────────────────
    public void NavigateTo<TView>(bool slideLeft = false)
        where TView : Control
    {
        NavigateInternal(typeof(TView), slideLeft, _ => { });
    }

    public void NavigateTo<TView, TParam>(TParam parameter, bool slideLeft = false)
        where TView : Control
        where TParam : INavigationParameter
    {
        NavigateInternal(typeof(TView), slideLeft, vm =>
        {
            if (vm is INavigationParameterReceiver<TParam> receiver)
                receiver.OnNavigatedTo(parameter);
        });
    }

    private void NavigateInternal(Type viewType, bool slideLeft, Action<object> configureVm)
    {
        if (!_registry.TryGetValue(viewType, out var descriptor))
            descriptor = AutoRegister(viewType);

        if (_currentView is not null && CurrentDescriptor is not null)
            _backStack.Push((CurrentDescriptor.ViewType, _currentView));

        Control view;
        object viewModel;

        // Cache tab pages, always create fresh detail pages
        if (descriptor.IsTab && _pageCache.TryGetValue(viewType, out var cached))
        {
            view = cached.View;
            viewModel = cached.ViewModel;
        }
        else
        {
            view = (Control)_serviceProvider.GetRequiredService(descriptor.ViewType);
            viewModel = _serviceProvider.GetRequiredService(descriptor.ViewModelType);
            view.DataContext = viewModel;

            if (descriptor.IsTab)
                _pageCache[viewType] = (view, viewModel);
        }

        configureVm(viewModel);

        if (_host is not null)
        {
            _host.PageTransition = new DirectionalPageSlide(slideLeft);
            _host.Content = view;
        }

        _currentView = view;
        CurrentDescriptor = descriptor;
        GoBackInternalCommand.NotifyCanExecuteChanged();
    }



    // ── Back navigation ──────────────────────────────────────────
    public bool CanGoBack => _backStack.Count > 0;

    public bool GoBack()
    {
        if (_backStack.Count == 0) return false;

        var (_, prevView) = _backStack.Pop();

        if (_host is not null)
        {
            _host.PageTransition = new DirectionalPageSlide(slideLeft: true);
            _host.Content = prevView;
        }

        _currentView = prevView;
        CurrentDescriptor = _registry.GetValueOrDefault(prevView.DataContext?.GetType()!);
        GoBackInternalCommand.NotifyCanExecuteChanged();
        return true;
    }

    // ── Commands ─────────────────────────────────────────────────
    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void GoBackInternal() => GoBack();

    IRelayCommand INavigationService.GoBackCommand => GoBackInternalCommand;
    
    [RelayCommand]
    private void NavigateToDescriptor(PageDescriptor descriptor)
    {
        NavigateInternal(descriptor.ViewType, false, _ => { });
    }

    IRelayCommand<PageDescriptor> INavigationService.NavigateToCommand => NavigateToDescriptorCommand;
}
