using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Mercury.Models;

namespace Mercury.Services.Interfaces;

public interface IAccountService : IServiceBase
{
    void AddAccount(OAuthTokenResponse oAuthToken);

    Task<DeviceCodeResponse> RequestDeviceCodeAsync(CancellationToken token = default);
    
    Task<OAuthTokenResponse> WaitForTokenAsync(DeviceCodeResponse deviceCode, CancellationToken token = default);
}