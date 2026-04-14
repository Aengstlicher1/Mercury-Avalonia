using CommunityToolkit.Mvvm.ComponentModel;

namespace Mercury.Services;

public interface ISearchService
{
    public string SearchQuery { get; set; }
}