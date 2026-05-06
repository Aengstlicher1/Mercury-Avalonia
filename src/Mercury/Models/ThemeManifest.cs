using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Mercury.Models;

public class ThemeManifest
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("author")]
    public string Author { get; init; } = string.Empty;
    
    [JsonPropertyName("version")]
    public Version Version { get; init; } = new Version();
    
    [JsonPropertyName("supports")]
    public string[] SupportedThemes { get; init; } = Array.Empty<string>();
    
    [JsonPropertyName("colorsLight")]
    public string LightPath { get; init; } = string.Empty;
    
    [JsonPropertyName("colorsDark")]
    public string DarkPath { get; init; } = string.Empty;
    
    [JsonPropertyName("styles")]
    public string[] StylesPaths { get; init; } = Array.Empty<string>();
    
    [JsonPropertyName("resources")]
    public string[] ResourcesPaths { get; init; } = Array.Empty<string>();
    
    [JsonPropertyName("preview")]
    public string PreviewPath { get; init; } = string.Empty;
}