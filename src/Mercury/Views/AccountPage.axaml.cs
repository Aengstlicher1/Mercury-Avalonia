using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mercury.Services.Implementations;
using Mercury.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Views;

public partial class AccountPage : ContentPage
{
    public AccountPage()
    {
        InitializeComponent();
    }

    private async void WebViewNavigated(object? sender, WebViewNavigationStartingEventArgs e)
    {
        if (e.Request is null) return;
        var host = e.Request!.Host.ToLowerInvariant();

        if (host.StartsWith("music.youtube.com"))
        {
            var accService = App.Services.GetRequiredService<IAccountService>();
            await accService.AddAccount(WebView.TryGetCookieManager()!);
        }
    }
}