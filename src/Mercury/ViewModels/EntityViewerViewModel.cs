using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Models.NavigationParameters;

namespace Mercury.ViewModels;

public partial class EntityViewerViewModel : ViewModelBase, INavigationParameterReceiver<EntityNavParameter>
{
    [ObservableProperty]
    public partial Entity Entity { get; set; }
    
    public void OnNavigatedTo(EntityNavParameter parameter)
    {
        if (parameter.Entity != null)
            Entity = parameter.Entity;
    }
}