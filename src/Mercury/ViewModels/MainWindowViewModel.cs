using System;
using System.Collections.Generic;
using System.Linq;
using static Mercury.Core.Models.Enums;

namespace Mercury.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public IEnumerable<SearchFilter> SearchFilters =>
        Enum.GetValues(typeof(SearchFilter)).Cast<SearchFilter>()
            .Where(s =>
                s != SearchFilter.Episodes &&
                s != SearchFilter.Profiles
            );

    public SearchFilter SelectedSearchFilter { get; set; } = SearchFilter.All;
}