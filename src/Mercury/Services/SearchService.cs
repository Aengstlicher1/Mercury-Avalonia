using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core.Models;

namespace Mercury.Services;

public partial class SearchService : ServiceBase, ISearchService 
{
    [ObservableProperty]
    private string _searchQuery = "";
    
    [ObservableProperty]
    private Enums.SearchFilter _searchFilter;
    public event Action<string, Enums.SearchFilter>? SearchParamChanged;

    partial void OnSearchQueryChanged(string value)
    {
        SearchParamChanged?.Invoke(value, SearchFilter);
    }
    partial void OnSearchFilterChanged(Enums.SearchFilter value)
    {
        SearchParamChanged?.Invoke(SearchQuery, value);
    }
    
    
    public override void OnExit()
    {
        
    }
}