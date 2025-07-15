using Assets.Scripts.GameLogic.models.creatures;
using Assets.Scripts.GameLogic.models.enums;
using Iterum.models.actions;
using Iterum.models.creatures;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Iterum.models.items
{
    public class WolfTeeth : IWeapon
    {
        public WolfTeeth()
        {
            WeaponActions.Add(new BasicMeleeWeaponAttack(this, "Bite"));
        }

        public IList<DamageInfo> DamageInfos { get; } = new List<DamageInfo> { new(2, Dice.d6, DamageType.Piercing) };

        public int ReachModifier { get; } = 0;

        public IList<WeaponTrait> WeaponTraits { get; } = new List<WeaponTrait>();

        public IList<IAction> WeaponActions { get; } = new List<IAction>();

        public WeaponSlotDetails WeaponSlotDetails { get; set; } = WeaponSlotDetails.Natural;

        public double Weight { get; } = 0;

        public string Name { get; } = "Wolf teeth";

        public Dictionary<Attribute, int> AttributeModifiers => new() { { Attribute.MaxHp, 2 } };
        public Dictionary<Attribute, double> AttributeMultipliers { get; } = new Dictionary<Attribute, double>();
        public ICreature Creature { get; set; }

        public WeaponType WeaponType { get; } = WeaponType.Natural;

        public bool CanEquip(ICreature creature)
        {
            return creature.GetType() == typeof(Wolf) || creature.GetType() == typeof(AlphaWolf);
        }
    }
}
