using System.Threading.Tasks;
using Mercury.Models;

namespace Mercury.Services;

public interface ISettingsService : IServiceBase
{
    PlayerSettings PlayerSettings { get; }
    DesignSettings DesignSettings { get; }
    
    Task Save();
    Task Load();
}