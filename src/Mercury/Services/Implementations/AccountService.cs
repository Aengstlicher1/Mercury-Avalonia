using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Mercury.Core;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Services.Interfaces;

namespace Mercury.Services.Implementations;

public class AccountService : ServiceBase, IAccountService
{
    private readonly HttpClient _client = new HttpClient();
    
    public List<IAuthSource> Accounts { get; } = new List<IAuthSource>();
    
    public void AddAccount(OAuthTokenResponse token)
    {
        var source = new OAuthSource(token.AccessToken, token.RefreshToken ?? String.Empty, DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn));
        Accounts.Add(source);
        YoutubeMusic.User.SetAuth(source);
    }

    public async Task<DeviceCodeResponse> RequestDeviceCodeAsync(CancellationToken ct)
    {
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = GoogleOAuthConstants.ClientId,
            ["scope"]     = GoogleOAuthConstants.Scope,
        });

        var resp = await _client.PostAsync(GoogleOAuthConstants.DeviceCodeUrl, form, ct);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<DeviceCodeResponse>(json)!;
    }
    
    public async Task<OAuthTokenResponse> WaitForTokenAsync(
    DeviceCodeResponse device, CancellationToken ct = default)
    {
        var interval = TimeSpan.FromSeconds(device.Interval);
        var deadline = DateTimeOffset.UtcNow.AddSeconds(device.ExpiresIn);

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"]     = GoogleOAuthConstants.ClientId,
            ["client_secret"] = GoogleOAuthConstants.ClientSecret,
            ["code"]          = device.DeviceCode,
            ["grant_type"]    = "http://oauth.net/grant_type/device/1.0",
        });
        
        while (DateTimeOffset.UtcNow < deadline)
        {
            await Task.Delay(interval, ct);
            
            var resp = await _client.PostAsync(GoogleOAuthConstants.TokenUrl, form, ct);
            var json = await resp.Content.ReadAsStringAsync(ct);

            if (!json.Contains("error"))
                return JsonSerializer.Deserialize<OAuthTokenResponse>(json)!;

            // Error responses are JSON: { "error": "authorization_pending" | "slow_down" | "access_denied" | "expired_token" }
            using var doc = JsonDocument.Parse(json);
            var error = doc.RootElement.GetProperty("error").GetString();

            switch (error)
            {
                case "authorization_pending": // user hasn't approved yet
                    break;

                case "slow_down": // Google wants us to slow down bump interval by 5s
                    interval += TimeSpan.FromSeconds(5);
                    break;

                case "access_denied":
                    throw new OperationCanceledException("User denied access.");

                case "expired_token":
                    throw new TimeoutException("Device code expired before user approved.");

                default:
                    throw new InvalidOperationException($"OAuth error: {error}");
            }
        }

        throw new TimeoutException("Device code expired before user approved.");
    }
    
    public override void OnExit()
    {
        
    }
}