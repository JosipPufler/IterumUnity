using Assets.Scripts;

namespace Iterum.models.interfaces
{
    public interface IGameObject : IGameEntity
    {
        GridCoordinate CurrentPosition { get; set; }
    }
}
