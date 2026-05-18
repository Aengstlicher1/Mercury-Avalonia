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
    public List<CookieAuthTokens> AuthTokens { get; } = new List<CookieAuthTokens>();
    public CookieAuthTokens? CurrentAuthTokens => YoutubeMusic.User.CurrentAuthTokens;
    
    public async Task AddAccount(NativeWebViewCookieManager manager)
    {
        var cookies = await manager.GetCookiesAsync();
        var wantedCookies = cookies
            .Where(x => x.Domain.Contains("youtube.com"))
            .ToDictionary(x => x.Name, x => x.Value);
        var tokens = new CookieAuthTokens(wantedCookies);

        /* Only add if the Cookies are not already saved */
        if (!AuthTokens.Any(t => t.Equals(tokens)))
        {
            AuthTokens.Add(tokens);
            YoutubeMusic.User.SetTokens(tokens);
        }
    }

    public override void OnExit()
    {
        
    }
}