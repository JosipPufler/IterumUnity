using Iterum.models.interfaces;

namespace Assets.Scripts.GameLogic.models.interfaces
{
    public class BaseItem : IItem
    {
        public virtual double Weight { get; set; }

        public virtual bool Stackable { get; set; }

        public virtual string Name { get; set; }
    }
}
