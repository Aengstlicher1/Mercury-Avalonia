using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Controls;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class PlaylistViewerViewModel : ViewModelBase, INavigationParameterReceiver<PlaylistNavParameter>
{
    private readonly IPlayerService _playerService = App.Services.GetRequiredService<IPlayerService>();
    private readonly INavigationService _navigationService = App.Services.GetRequiredService<INavigationService>();
    
    [ObservableProperty]
    public partial PlaylistInfo? Info { get; set; }
    
    [ObservableProperty]
    public partial Track? CurrentTrack { get; set; }
    
    
    public void OnNavigatedTo(PlaylistNavParameter parameter)
    {
        Task.Run(async () =>
        {
            var info = await YoutubeMusic.Browse.GetInfoAsync(parameter.Playlist!);
            await Dispatcher.UIThread.InvokeAsync(() => Info = info as PlaylistInfo);
        });
    }


    [RelayCommand]
    private void SetPlaylist()
    {
        if (Info != null)
            _playerService.SetPlaylist(Info);
    }
    
    [RelayCommand]
    private void PlayTrack(Track track)
    {
        _playerService.SetTrack(track);
    }
    
    [RelayCommand]
    private void NavigateToEntity(Entity? entity)
    {
        _navigationService.NavigateTo<EntityViewer, EntityNavParameter>(new EntityNavParameter(entity));
    }
}