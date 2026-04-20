using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Mercury.Models;

public partial class PageInfo(Page page) : ObservableObject
{
    public string Name => Page.Name ?? string.Empty;
    public Page Page { get; init; } = page;
    
    [ObservableProperty] 
    private bool _isSelected = false;
}