using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Core.Models;
using Mercury.Services;
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

    [ObservableProperty] private Track? _currentTrack;

    partial void OnCurrentTrackChanged(Track? value)
    {
        _playerService.CurrentTrack = value;
    }
}
