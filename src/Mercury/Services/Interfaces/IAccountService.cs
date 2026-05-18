using System.Threading.Tasks;
using Avalonia.Controls;

namespace Mercury.Services.Interfaces;

public interface IAccountService : IServiceBase
{
    Task AddAccount(NativeWebViewCookieManager manager);
}