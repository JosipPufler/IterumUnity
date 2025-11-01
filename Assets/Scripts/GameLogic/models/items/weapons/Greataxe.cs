using Assets.Scripts.GameLogic.models.enums;
using Iterum.models;
using Iterum.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using Attribute = Iterum.models.enums.Attribute;

namespace Assets.Scripts.GameLogic.models.items.weapons
{
    public class Greataxe : BaseWeapon
    {
        public Greataxe(bool init) : base(init){}

        public override void Initialize()
        {
            Actions.Add(new BasicMeleeWeaponAttack(this, "Swing"));
        }
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public override IList<DamageInfo> DamageInfos { get; set; } = new List<DamageInfo> { new(1, Dice.d12, DamageType.Slashing) };
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public override IList<WeaponTrait> WeaponTraits { get; set; } = new List<WeaponTrait>() { WeaponTrait.Heavy };

        public override WeaponSlotDetails WeaponSlotDetails { get; set; } = WeaponSlotDetails.TwoHand;

        public override string Name { get; set; } = "Greataxe";

        public override WeaponType WeaponType { get; set; } = WeaponType.Axe;

        public override string Description { get; set; } = "A heavy two-handed axe forged for war. Deals 1d12 slashing damage.";
        public override bool CanEquip(BaseCreature creature)
        {
            return creature.GetAttributeModifier(Attribute.Strength) >= 2;
        }
    }
}
