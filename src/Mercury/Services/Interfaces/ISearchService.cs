using System;
using Mercury.Core.Models;

namespace Mercury.Services.Interfaces;

public interface ISearchService : IServiceBase
{
    string SearchQuery { get; set; }
    Enums.SearchFilter SearchFilter { get; set; }
    
    event Action<string, Enums.SearchFilter>? SearchParamChanged;
}