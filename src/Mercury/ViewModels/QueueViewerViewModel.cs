using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Controls;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class QueueViewerViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService = App.Services.GetRequiredService<IPlayerService>();
    private readonly INavigationService _navigationService = App.Services.GetRequiredService<INavigationService>();

    [ObservableProperty] 
    public partial Track? CurrentTrack { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<Track> Queue { get; set; }
    
    public QueueViewerViewModel()
    {
        _playerService.QueueChanged += tracks => Queue = new ObservableCollection<Track>(tracks);
        _playerService.CurrentTrackChanged += track => CurrentTrack = track;
        Queue = _playerService.CurrentQueue;
    }

    [RelayCommand]
    private void PlayTrack(Track track)
    {
        _playerService.SetPlaylistTrack(track);
    }

    [RelayCommand]
    private void NavigateToEntity(Entity entity)
    {
        _navigationService.NavigateTo<EntityViewer, EntityNavParameter>(new EntityNavParameter(entity));
    }
}