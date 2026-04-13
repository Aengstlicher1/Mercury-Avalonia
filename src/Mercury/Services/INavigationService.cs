using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Mercury.Models;

namespace Mercury.Services;

public interface INavigationService
{
    public INavigation Navigation { get; internal set; }
    public PageInfo? CurrentPageInfo { get; set; }
    public Page? CurrentPage { get; }
    
    public static ContentPage[] Pages { get; }
    public PageInfo[] PageInfos { get; }

    public Task NavigateTo(PageInfo pageInfo);
    public IAsyncRelayCommand<PageInfo> NavigateToCommand { get; }
}