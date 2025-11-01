using Assets.Scripts.GameLogic.models.interfaces;

namespace Iterum.models.interfaces
{
    public interface IItem : IGameEntity, ITooltipTarget
    {
        string ID { get; }
        double Weight { get; }

        bool Stackable { get; }
        string Description { get; }
    }
}
