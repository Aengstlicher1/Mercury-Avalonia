using CommunityToolkit.Mvvm.ComponentModel;
using Mercury.Services.Interfaces;

namespace Mercury.Services.Implementations;

public abstract class ServiceBase : ObservableObject, IServiceBase
{
    public abstract void OnExit();
}