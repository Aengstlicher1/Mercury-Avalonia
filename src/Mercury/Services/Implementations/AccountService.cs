using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Services.Interfaces;

namespace Mercury.Services.Implementations;

public class AccountService : ServiceBase, IAccountService
{
    public List<CookieAuthTokens> Accounts { get; } = new List<CookieAuthTokens>();
    
    public async Task AddAccount(NativeWebViewCookieManager manager)
    {
        var cookies = await manager.GetCookiesAsync();
        var wantedCookies = cookies
            .Where(x => x.Domain.Contains("youtube.com"))
            .ToDictionary(x => x.Name, x => x.Value);
        var tokens = new CookieAuthTokens(wantedCookies);

        /* Only add if the Cookies are not already saved */
        if (!Accounts.Any(t => t.Equals(tokens)))
        {
            Accounts.Add(tokens);
            YoutubeMusic.User.SetTokens(tokens);
        }
    }

    public override void OnExit()
    {
        
    }
}