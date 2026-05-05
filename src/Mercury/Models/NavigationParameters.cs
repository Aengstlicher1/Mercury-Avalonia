using Mercury.Core.Models;

namespace Mercury.Models;

public record EntityNavParameter(Entity? Entity) : INavigationParameter
{
    public readonly Entity? Entity = Entity;
}

public record PlaylistNavParameter(Playlist? Playlist) : INavigationParameter
{
    public readonly Playlist? Playlist = Playlist;
}