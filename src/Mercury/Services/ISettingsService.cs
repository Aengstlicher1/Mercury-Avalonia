using Mercury.Models;

namespace Mercury.Services;

public interface ISettingsService : IServiceBase
{
    PlayerSettings PlayerSettings { get; }
    DesignSettings DesignSettings { get; }
    
    void Save();
    void Load();
}