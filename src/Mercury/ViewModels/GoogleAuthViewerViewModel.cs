using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Services.Interfaces;
using Mercury.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class GoogleAuthViewerViewModel : ViewModelBase
{
    private readonly IAccountService _accountService = App.Services.GetRequiredService<IAccountService>();
    private readonly INavigationService _navigationService = App.Services.GetRequiredService<INavigationService>();
    
    private CancellationTokenSource _loginCts = new();
    
    [ObservableProperty]
    public partial string? FormatedCode { get; set; }

    public GoogleAuthViewerViewModel()
    {
        Login();
    }

    private async Task StartLogin(CancellationToken cToken = default)
    {
        if (Design.IsDesignMode) return;
        
        var deviceCode = await _accountService.RequestDeviceCodeAsync(cToken);
        FormatedCode = deviceCode.UserCode;
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://www.google.com/device",
            UseShellExecute = true
        }); // Open the website in the default Browser

        var token = await _accountService.WaitForTokenAsync(deviceCode, cToken);
        _accountService.AddAccount(token);
        _navigationService.NavigateTo<AccountPage>();
    }
    
    private void Login()
    {
        _loginCts.Cancel();
        _loginCts.Dispose();
        _loginCts = new CancellationTokenSource();
        _ = StartLogin(_loginCts.Token);
    }
}