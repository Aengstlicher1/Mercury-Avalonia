using Mercury.Core.Models;

namespace Mercury.Models.NavigationParameters;

public record EntityNavParameter(Entity Entity) : INavigationParameter
{
    public readonly Entity? Entity = Entity;
}