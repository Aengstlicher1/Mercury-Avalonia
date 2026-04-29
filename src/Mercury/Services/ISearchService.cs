using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core.Models;

namespace Mercury.Services;

public interface ISearchService : IServiceBase
{
    string SearchQuery { get; set; }
    Enums.SearchFilter SearchFilter { get; set; }
    
    event Action<string, Enums.SearchFilter>? SearchParamChanged;
}