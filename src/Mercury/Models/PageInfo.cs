using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Mercury.Models;

public partial class PageInfo : ObservableObject
{
    public required string Name { get; set; }
    public required Page Page { get; init; }
    
    [ObservableProperty] 
    private bool _isSelected = false;
}