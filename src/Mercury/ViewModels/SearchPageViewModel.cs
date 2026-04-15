using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Services;
using Microsoft.Extensions.DependencyInjection;


namespace Mercury.ViewModels;

public partial class SearchPageViewModel : ViewModelBase
{
    private CancellationTokenSource _cts = new();
    private IPlayerService _ps;
    public ObservableCollection<Media> SearchResults { get; set; } = new ();

    public SearchPageViewModel()
    {
        _ps = App.Services.GetRequiredService<IPlayerService>();
        
        var searchService = App.Services.GetRequiredService<ISearchService>();
        searchService.SearchParamChanged += (query, filter) =>
        {
            _ = PerformSearch(query, filter);
        };
    }

    private async Task PerformSearch(string query, Enums.SearchFilter filter)
    {
        await _cts.CancelAsync();
        _cts = new CancellationTokenSource();

        if (string.IsNullOrEmpty(query))
        {
            SearchResults.Clear();
            return;
        }
        
        await Task.Delay(240, _cts.Token);

        _cts.Token.ThrowIfCancellationRequested();
        
        var results = filter is Enums.SearchFilter.All
            ? await YoutubeMusic.Search.SearchAsync(query)
            : await YoutubeMusic.Search.SearchCategoryAsync(query, filter);

        if (results != null && results.Any())
        {
            SearchResults.Clear();
        
            foreach (var result in results)
            {
                SearchResults.Add(result);
                Debug.WriteLine($"Added Search Result: {result.Title}");
            }
        }
    }

    [RelayCommand]
    private void PlayTrack(Track track)
    {
        _ = _ps.SetTrack(track);
        Debug.WriteLine($"Playing Track: {track.Title}");
    }
}