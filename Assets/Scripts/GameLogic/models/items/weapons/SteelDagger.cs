using Assets.Scripts.GameLogic.models.enums;
using Iterum.models;
using Iterum.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.items.weapons
{
    public class SteelDagger : BaseWeapon
    {
        public SteelDagger(bool init) : base(init)
        {
            Actions.Add(new BasicMeleeWeaponAttack(this, "Stab"));
        }
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public override IList<DamageInfo> DamageInfos { get; set; } = new List<DamageInfo> { new(1, Dice.d4, DamageType.Piercing) };
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public override IList<WeaponTrait> WeaponTraits { get; set; } = new List<WeaponTrait>() { WeaponTrait.Light, WeaponTrait.Finesse };

        public override WeaponSlotDetails WeaponSlotDetails { get; set; } = WeaponSlotDetails.OneHand;

        public override string Name { get; set; } = "Steel dagger";

        public override WeaponType WeaponType { get; set; } = WeaponType.Dagger;

        public override bool CanEquip(BaseCreature creature)
        {
            return true;
        }
    }
}
