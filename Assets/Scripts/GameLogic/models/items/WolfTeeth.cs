using Assets.Scripts.GameLogic.models.creatures;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.items;
using Iterum.models.actions;
using Iterum.models.creatures;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Iterum.models.items
{
    public class WolfTeeth : BaseWeapon
    {
        public WolfTeeth()
        {
            WeaponActions.Add(new BasicMeleeWeaponAttack(this, "Bite"));
        }

        public override void Initialize()
        {
        }

        public override IList<DamageInfo> DamageInfos { get; set; } = new List<DamageInfo> { new(2, Dice.d6, DamageType.Piercing) };

        public override IList<WeaponTrait> WeaponTraits { get; set; } = new List<WeaponTrait>() { WeaponTrait.Natural};

        public override WeaponSlotDetails WeaponSlotDetails { get; set; } = WeaponSlotDetails.Natural;

        public override string Name { get; set; } = "Wolf teeth";

        public override Dictionary<Attribute, int> AttributeModifiers => new() { { Attribute.MaxHp, 2 } };
        
        public override WeaponType WeaponType { get; set; } = WeaponType.Natural;

        public override bool CanEquip(BaseCreature creature)
        {
            return creature.GetType() == typeof(Wolf) || creature.GetType() == typeof(AlphaWolf);
        }
    }
}
