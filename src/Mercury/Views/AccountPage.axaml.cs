using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Shared;
using Mercury.Services.Implementations;
using Mercury.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class AccountPage : ContentPage
{
    private readonly IAccountService _accountService;
    
    public AccountPage()
    {
        InitializeComponent();
        _accountService = App.Services.GetRequiredService<IAccountService>();
    }
}