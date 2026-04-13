using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Mercury.Models;

public partial class PageInfo : ObservableObject
{
    public string Name { get; set; }
    public Page Page { get; set; }
    
    [ObservableProperty] 
    private bool _isSelected = false;
}