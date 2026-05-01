using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core.Models;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.ViewModels;

public partial class QueueViewerViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService = App.Services.GetRequiredService<IPlayerService>();
    
    public ObservableCollection<Track> Queue => _playerService.CurrentQueue;
}