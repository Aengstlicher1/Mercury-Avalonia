using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using IconPacks.Avalonia.MaterialDesign;
using Mercury.Models;

namespace Mercury.Services;

public interface INavigationService : IServiceBase
{
    PageDescriptor? CurrentDescriptor { get; }
    IReadOnlyList<PageDescriptor> Tabs { get; }

    void SetHost(TransitioningContentControl host);
    
    void Register<TView, TViewModel>(string title, PackIconMaterialDesignKind icon, bool isTab = false)
        where TView : Avalonia.Controls.Control
        where TViewModel : class;

    void NavigateTo<TView>(bool slideLeft = false)
        where TView : Avalonia.Controls.Control;

    void NavigateTo<TView, TParam>(TParam parameter, bool slideLeft = false)
        where TView : Avalonia.Controls.Control
        where TParam : INavigationParameter;

    bool GoBack();
    bool CanGoBack { get; }

    IRelayCommand GoBackCommand { get; }
    IRelayCommand<PageDescriptor> NavigateToCommand { get; }
}