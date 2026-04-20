using System.Collections.Generic;
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
    private readonly IPlayerService _ps;
    public ObservableCollection<Media> SearchResults { get; set; } = new ();

    public SearchPageViewModel()
    {
        _ps = App.Services.GetRequiredService<IPlayerService>();
        
        var searchService = App.Services.GetRequiredService<ISearchService>();
        
        // redo Search on query or filter change
        searchService.SearchParamChanged += (query, filter) =>
        {
            _ = PerformSearch(query, filter);
        };
    }

    private async Task PerformSearch(string query, Enums.SearchFilter filter)
    {
        // refresh CTS
        await _cts.CancelAsync();
        _cts = new CancellationTokenSource();

        if (string.IsNullOrEmpty(query))
        {
            SearchResults.Clear();
            return;
        }
        
        // small delay to let the user type without spamming the API
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
            }

            // if we do not play from a playlist currently, make the Search the Queue
            if (_ps.CurrentPlaylist == null)
            {
                _ps.CurrentQueue.Clear();
                foreach (var result in results.OfType<Track>())
                {
                    _ps.CurrentQueue.Add(result);
                }
            }
        }
    }

    [RelayCommand]
    private void PlayTrack(Track track)
    {
        // Don't wait to keep UI responsive and active
        _ = _ps.SetTrack(track);
    }
}