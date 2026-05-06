using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Services.Interfaces;

namespace Mercury.Services;

public abstract class ServiceBase : ObservableObject, IServiceBase
{
    public abstract void OnExit();
}