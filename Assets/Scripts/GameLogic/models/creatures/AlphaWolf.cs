using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.models.items;
using Iterum.models.races;

namespace Assets.Scripts.GameLogic.models.creatures
{
    public class AlphaWolf : BaseCreature
    {
        public new static string DisplayName { get; private set; } = "Alpha wolf";

        public AlphaWolf() : base(new Wolf(), DisplayName, "Textures/alphawolf")
        {
            WeaponSet.AddWeapon(new WolfTeeth(true));
        }

        public AlphaWolf(bool init) : this()
        {
            if (init)
            {
                Race.RacialAttributes[Attribute.MaxHp] = 40;
                CurrentHp = MaxHp;
                ModifierManager.AddModifier(Attribute.MaxAp, 2);
                ModifierManager.AddModifier(Attribute.Strength, 4);
            }
        }
    }
}
