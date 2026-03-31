using System.Net.Http.Headers;

internal sealed class Client : Dictionary<string, object>
{
    public static readonly Client WebMusic = new()
    {
        ["hl"] = "en",
        ["gl"] = "US",
        ["platform"] = "DESKTOP",
        ["clientName"] = "WEB_REMIX",
        ["clientVersion"] = "1.20251111.09.00",
        ["deviceMake"] = "",
        ["deviceModel"] = "",
        ["osName"] = "Windows",
        ["osVersion"] = "10.0",
        ["userAgent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36",
        ["browserName"] = "Chrome",
        ["browserVersion"] = "142.0.0.0",
        ["originalUrl"] = "https://music.youtube.com/",
        ["clientFormFactor"] = "UNKNOWN_FORM_FACTOR",
        ["screenHeightPoints"] = 1440,
        ["screenWidthPoints"] = 2560,
        ["screenDensityFloat"] = 1f,
        ["screenPixelDensity"] = 1,
        ["userInterfaceTheme"] = "USER_INTERFACE_THEME_DARK",
        ["timeZone"] = "UTC",
        ["utcOffsetMinutes"] = 0
    };

    public static readonly Client IOSMusic = new()
    {
        ["hl"] = "en",
        ["gl"] = "US",
        ["platform"] = "MOBILE",
        ["clientName"] = "IOS_MUSIC",
        ["clientVersion"] = "7.21.50",
        ["clientFormFactor"] = "UNKNOWN_FORM_FACTOR",
        ["deviceMake"] = "Apple",
        ["deviceModel"] = "iPhone16,2",
        ["osName"] = "iOS",
        ["osVersion"] = "18.1.0.22B83",
        ["userAgent"] = "com.google.ios.youtube.music/7.21.50 (iPhone16,2; U; CPU iOS 18_1_0 like Mac OS X; US)",
        ["screenHeightPoints"] = 1440,
        ["screenWidthPoints"] = 2560,
        ["screenDensityFloat"] = 1f,
        ["screenPixelDensity"] = 1,
        ["userInterfaceTheme"] = "USER_INTERFACE_THEME_DARK",
        ["timeZone"] = "UTC",
        ["utcOffsetMinutes"] = 0
    };

    public static readonly Client Web = new()
    {
        ["hl"] = "en",
        ["gl"] = "US",
        ["platform"] = "DESKTOP",
        ["clientName"] = "WEB",
        ["clientVersion"] = "2.20250312.04.00",
        ["deviceMake"] = "",
        ["deviceModel"] = "",
        ["osName"] = "Windows",
        ["osVersion"] = "10.0",
        ["userAgent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36",
        ["browserName"] = "Chrome",
        ["browserVersion"] = "142.0.0.0",
        ["originalUrl"] = "https://www.youtube.com/",
        ["clientFormFactor"] = "UNKNOWN_FORM_FACTOR",
        ["screenHeightPoints"] = 1440,
        ["screenWidthPoints"] = 2560,
        ["screenDensityFloat"] = 1f,
        ["screenPixelDensity"] = 1,
        ["userInterfaceTheme"] = "USER_INTERFACE_THEME_DARK",
        ["timeZone"] = "UTC",
        ["utcOffsetMinutes"] = 0,
        ApiKey = "?key=AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8"
    };

    public static readonly Client Android = new()
    {
        ["hl"] = "en",
        ["gl"] = "US",
        ["platform"] = "MOBILE",
        ["clientName"] = "ANDROID",
        ["clientVersion"] = "19.29.37",
        ["deviceMake"] = "Google",
        ["deviceModel"] = "Pixel 8",
        ["osName"] = "Android",
        ["osVersion"] = "14",
        ["androidSdkVersion"] = 34,
        ["userAgent"] = "com.google.android.youtube/19.29.37 (Linux; U; Android 14; US) gzip",
        ["timeZone"] = "UTC",
        ["utcOffsetMinutes"] = 0,
        Headers = [ new("X-Goog-Api-Format-Version", "2") ]
    };

    public static readonly Client AndroidVR = new()
    {
        ["hl"] = "en",
        ["gl"] = "US",
        ["platform"] = "MOBILE",
        ["clientName"] = "ANDROID_VR",
        ["clientVersion"] = "1.60.19",
        ["deviceMake"] = "Meta",
        ["deviceModel"] = "Quest 3",
        ["osName"] = "Android",
        ["osVersion"] = "12L",
        ["androidSdkVersion"] = 32,
        ["userAgent"] = "com.google.android.apps.youtube.vr.oculus/1.60.19 (Linux; U; Android 12L; US) gzip",
        ["timeZone"] = "UTC",
        ["utcOffsetMinutes"] = 0,
        ApiKey = "?key=AIzaSyA8eiZmM1FaDVjRy-df2KTyQ_vz_yYM39wnew"
    };

    // ── Convenience Properties ───────────────────────────────────────────

    public KeyValuePair<string, string>[]? Headers
    {
        get;
        set;
    }

    public string ApiKey { get; init; } = "";
    public string VisitorData
    {
        get => (string)this["visitorData"];
        set => this["visitorData"] = value;
    }
    public string Hl
    {
        get => (string)this["hl"];
        set => this["hl"] = value;
    }

    public string Gl
    {
        get => (string)this["gl"];
        set => this["gl"] = value;
    }

    public string Platform
    {
        get => (string)this["platform"];
        set => this["platform"] = value;
    }

    public string ClientName
    {
        get => (string)this["clientName"];
        set => this["clientName"] = value;
    }

    public string ClientVersion
    {
        get => (string)this["clientVersion"];
        set => this["clientVersion"] = value;
    }

    public string UserAgent
    {
        get => (string)this["userAgent"];
        set => this["userAgent"] = value;
    }

    public string TimeZone
    {
        get => (string)this["timeZone"];
        set => this["timeZone"] = value;
    }

    public int UtcOffsetMinutes
    {
        get => (int)this["utcOffsetMinutes"];
        set => this["utcOffsetMinutes"] = value;
    }

    // ── Methods ──────────────────────────────────────────────────────────

    public Client Clone()
    {
        var copy = new Client();
        foreach (var kvp in this)
            copy[kvp.Key] = kvp.Value;
        copy.Headers = Headers?.ToArray();
        return copy;
    }
}
