using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.interfaces;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Assets.Scripts.GameLogic.models.items
{
    public class BaseWeapon : BaseEquipment, IWeapon
    {
        public BaseWeapon()
        {
            Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Initialize();
        }

        public virtual void Initialize() { }

        public virtual IList<DamageInfo> DamageInfos { get; set; } = new List<DamageInfo>();

        public virtual int EvasionRatingBonus
        {
            get => 0;
        }

        public virtual int ReachModifier { get; set; }

        public virtual IList<WeaponTrait> WeaponTraits { get; set; } = new List<WeaponTrait>();

        public virtual WeaponType WeaponType { get; set; }

        public virtual IList<BaseAction> WeaponActions { get; set; } = new List<BaseAction>();

        public virtual WeaponSlotDetails WeaponSlotDetails { get; set; }

        [JsonIgnore]
        public virtual BaseCreature Creature { get; set; }

        public override bool CanEquip(BaseCreature creature) {
            return true;
        }

        public string GetDamageInfoString()
        {
            if (DamageInfos.Count == 1)
                return DamageInfos[0].ToString();

            if (DamageInfos.Count == 2)
                return $"{DamageInfos[0]} and {DamageInfos[1]}";

            var allButLast = DamageInfos
                .Take(DamageInfos.Count - 1)
                .Select(di => di.ToString());
            var last = DamageInfos[DamageInfos.Count - 1].ToString();

            return $"{string.Join(", ", allButLast)} and {last}";
        }
    }
}
