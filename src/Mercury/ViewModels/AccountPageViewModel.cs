using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Mercury.Controls;
using Mercury.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class AccountPageViewModel(INavigationService navigationService) : ViewModelBase
{
    [RelayCommand]
    private void RedirectToLogin()
    {
        navigationService.NavigateTo<GoogleAuthViewer>();
    }
}