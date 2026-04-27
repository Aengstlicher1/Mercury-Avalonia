using System;
using Mercury.Core.Models;

namespace Mercury.Services;

public interface ILyricService
{
    Lyrics Lyrics { get; }
    
    event Action<Lyrics?> LyricsChanged;
}