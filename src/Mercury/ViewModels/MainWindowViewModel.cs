using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Mercury.Core.Models;

namespace Mercury.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public IEnumerable<Enums.SearchFilter> SearchFilters =>
        Enum.GetValues(typeof(Enums.SearchFilter)).Cast<Enums.SearchFilter>()
            .Where(s =>
                s != Enums.SearchFilter.Episodes &&
                s != Enums.SearchFilter.Profiles
            );
    
}