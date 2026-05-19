using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Mercury.Models;

internal static class GoogleOAuthConstants
{
    public const string ClientId = "861556708454-d6dlm3lh05idd8npek18k6be8ba3oc68.apps.googleusercontent.com";
    public const string ClientSecret = "SboVhoG9s0rNafixCSGGKXAT";
    public const string Scope = "https://www.googleapis.com/auth/youtube";

    public const string DeviceCodeUrl = "https://www.youtube.com/o/oauth2/device/code";
    public const string TokenUrl = "https://www.youtube.com/o/oauth2/token";
}

public sealed record DeviceCodeResponse
{
    [property: JsonPropertyName("device_code")]
    public string DeviceCode { get; init; } = string.Empty;
    
    [property: JsonPropertyName("user_code")]
    public string UserCode { get; init; } = string.Empty;
    
    [property: JsonPropertyName("verification_url")]
    public string VerificationUrl { get; init; } = string.Empty;
    
    [property: JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
    
    [property: JsonPropertyName("interval")]
    public int Interval { get; init; }
}

public sealed record OAuthTokenResponse
{
    [property: JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;
    
    [property: JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
    
    [property: JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
    
    [property: JsonPropertyName("token_type")]
    public string TokenType { get; init; } = string.Empty;

    [property: JsonPropertyName("scope")] 
    public string Scope { get; init; } = string.Empty;
}