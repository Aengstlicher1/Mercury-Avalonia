using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core.Models;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class QueueViewerViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService = App.Services.GetRequiredService<IPlayerService>();
    
    [ObservableProperty]
    public partial ObservableCollection<Track> Queue { get; set; } = new();
    
    public QueueViewerViewModel()
    {
        _playerService.QueueChanged += _ => Queue = _playerService.CurrentQueue;
        Queue = _playerService.CurrentQueue;
    }
}