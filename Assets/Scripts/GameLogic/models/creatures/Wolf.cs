using Iterum.models.interfaces;
using Iterum.models.items;

namespace Iterum.models.creatures
{
    public class Wolf : BaseCreature
    {
        public new static string DisplayName { get; private set; } = "Wolf";

        public Wolf() : base(new races.Wolf(), DisplayName, "Textures/wolf")
        {
            WeaponSet.AddWeapon(new WolfTeeth(true));
        }

        public Wolf(bool init) : this() {
            if (init) 
                CurrentHp = MaxHp;
        }
    }
}
