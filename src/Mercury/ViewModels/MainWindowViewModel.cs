using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;
using static Mercury.Core.Models.Enums;

namespace Mercury.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private HttpClient _client = new HttpClient();
    
    public IEnumerable<SearchFilter> SearchFilters =>
        Enum.GetValues(typeof(SearchFilter)).Cast<SearchFilter>()
            .Where(s =>
                s != SearchFilter.Episodes &&
                s != SearchFilter.Profiles
            );

    public SearchFilter SelectedSearchFilter { get; set; } = SearchFilter.All;
    
    [ObservableProperty]
    private Track? _currentTrack;

    [ObservableProperty]
    private IImage? _currentTrackImage;
    
    [ObservableProperty]
    private IImage? _currentBackgroundImage;
    
    private readonly INavigationService _navigationService;
    private readonly ISearchService _searchService;
    public PageInfo[] PageInfos => _navigationService.PageInfos;
    public IRelayCommand<PageInfo> NavigateToCommand =>  _navigationService.NavigateToCommand;
    
    public MainWindowViewModel()
    {
        _navigationService = App.Services.GetRequiredService<INavigationService>();
        _searchService = App.Services.GetRequiredService<ISearchService>();
        _navigationService.NavigateTo(_navigationService.PageInfos[0]);
    }
    
    
    private async Task<Bitmap> LoadTrackImageAsync(string url)
    {
        var bytes = await _client.GetByteArrayAsync(url);
        using var ms = new MemoryStream(bytes);
        return new Bitmap(ms);
    }

    async partial void OnCurrentTrackChanged(Track? value)
    {
        if (value!.Thumbnails.TryGetWithSize(new Dimensions(240, 240), true, out var thumbnail))
        {
            CurrentBackgroundImage = await LoadTrackImageAsync(thumbnail.Url);
            CurrentTrackImage = CurrentBackgroundImage;
        }
    }

    [ObservableProperty]
    private string _searchText = "";

    partial void OnSearchTextChanged(string value)
    {
        _searchService.SearchQuery = value;
    }
}