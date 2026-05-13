using System;
using Avalonia.Media.Fonts;

namespace Mercury.Models;

public class MercuryFontCollection : EmbeddedFontCollection
{
    public MercuryFontCollection() : base(new Uri("fonts:Mercury", UriKind.Absolute), new Uri("avares://Mercury/Assets/Fonts"))
    {
        
    }
}