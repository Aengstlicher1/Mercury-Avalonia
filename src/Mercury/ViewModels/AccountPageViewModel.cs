using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Mercury.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class AccountPageViewModel : ViewModelBase
{
    public static string LoginUrl { get; } =
        "https://accounts.google.com/ServiceLogin?service=youtube&continue=https%3A%2F%2Fmusic.youtube.com%2F";
}