using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Core.Models;
using Mercury.Services;
using Mercury.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class PlayingPageViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService;

    public PlayingPageViewModel()
    {
        _playerService = App.Services.GetRequiredService<IPlayerService>();
        _playerService.CurrentTrackChanged += track =>
            Dispatcher.UIThread.Post(() => CurrentTrack = track);

        CurrentTrack = _playerService.CurrentTrack;
    }

    [ObservableProperty]
    public partial Track? CurrentTrack { get; set; }
}
