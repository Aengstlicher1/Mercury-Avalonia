using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;

namespace Mercury.Resources;

public class DirectionalPageSlide : IPageTransition
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(100);
    public bool SlideLeft { get; set; }

    public async Task Start(Visual? from, Visual? to, bool forward, 
        CancellationToken cancellationToken)
    {
        // Use a standard PageSlide but flip "forward" based on direction
        var slide = new PageSlide(Duration);
        await slide.Start(from, to, !SlideLeft, cancellationToken);
    }
}
