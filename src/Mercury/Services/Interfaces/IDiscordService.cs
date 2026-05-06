namespace Mercury.Services.Interfaces;

public interface IDiscordService : IServiceBase
{
    void Initialize();
    void ClearPresence();
}