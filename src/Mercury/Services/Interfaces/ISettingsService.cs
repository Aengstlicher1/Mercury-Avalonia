using System.Threading.Tasks;
using Mercury.Models;

namespace Mercury.Services.Interfaces;

public interface ISettingsService : IServiceBase
{
    PlayerSettings PlayerSettings { get; }
    DesignSettings DesignSettings { get; }
    
    void Save();
    void Load();
}