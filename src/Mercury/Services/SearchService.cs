using CommunityToolkit.Mvvm.ComponentModel;

namespace Mercury.Services;

public partial class SearchService : ServiceBase, ISearchService 
{
    [ObservableProperty]
    private string _searchQuery = "";

    partial void OnSearchQueryChanged(string value)
    {
        throw new System.NotImplementedException();
    }
}