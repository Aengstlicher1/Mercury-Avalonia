using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core.Models;

namespace Mercury.ViewModels;

public partial class LyricLineViewModel(LyricLine line) : ViewModelBase
{
    public LyricLine Line { get; } = line;

    // Expose the text directly for binding
    public string Content => Line.Content;

    // The timestamp when this line starts
    public TimeSpan Timing => Line.Timing;

    [ObservableProperty]
    private LyricState _state = LyricState.Upcoming;
    
    public bool IsCurrent => State == LyricState.Current;
    public bool IsPast => State == LyricState.Past;
    
    
    partial void OnStateChanged(LyricState value)
    {
        OnPropertyChanged(nameof(IsCurrent));
        OnPropertyChanged(nameof(IsPast));
    }
}

public enum LyricState : short
{
    Upcoming = 0,
    Current = 1,
    Past = 2
}