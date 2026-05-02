using System;

namespace Mercury.Services;

public interface IDiscordService : IServiceBase
{
    void Initialize();
    void ClearPresence();
}