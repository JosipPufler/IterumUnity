namespace Iterum.models.interfaces
{
    public interface IItem : IGameEntity
    {
        double Weight { get; }

        bool Stackable { get; }
    }
}
