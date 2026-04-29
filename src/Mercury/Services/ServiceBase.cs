using CommunityToolkit.Mvvm.ComponentModel;

namespace Mercury.Services;

public abstract class ServiceBase : ObservableObject, IServiceBase
{
    public abstract void OnExit();
}