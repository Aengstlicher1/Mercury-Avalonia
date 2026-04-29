using System;
using Mercury.Core.Models;

namespace Mercury.Services;

public interface ILyricService : IServiceBase
{
    Lyrics Lyrics { get; }
    
    event Action<Lyrics?> LyricsChanged;
}