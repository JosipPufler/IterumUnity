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
using System.Text;
using Unity.VisualScripting;

namespace Assets.Scripts.GameLogic.models.items
{
    public class BaseWeapon : BaseEquipment, IWeapon
    {
        public BaseWeapon(bool init){
            if (init)
            {
                Initialize();
            }
        }

        [JsonConstructor]
        public BaseWeapon() { }

        public BaseWeapon(BaseWeapon weapon)
        {
            Name = weapon.Name;
            Description = weapon.Description;
            WeaponSlotDetails = weapon.WeaponSlotDetails;
            WeaponTraits = weapon.WeaponTraits;
            WeaponType = weapon.WeaponType;
            DamageInfos = weapon.DamageInfos;
            EvasionRatingBonus = weapon.EvasionRatingBonus;
            Actions = weapon.Actions.Select(a => a.Clone()).ToList();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            foreach (var action in Actions)
            {
                if (action is WeaponAction weaponAction)
                {
                    weaponAction.Weapon = this;
                }
            }
        }

        public virtual void Initialize() { }

        public virtual IList<DamageInfo> DamageInfos { get; set; } = new List<DamageInfo>();

        public virtual int EvasionRatingBonus { get; set; } = 0;

        public virtual int ReachModifier { get => WeaponTraits.Contains(WeaponTrait.Reach) ? 2 : 1; }

        public virtual IList<WeaponTrait> WeaponTraits { get; set; } = new List<WeaponTrait>();

        public virtual WeaponType WeaponType { get; set; }

        public virtual WeaponSlotDetails WeaponSlotDetails { get; set; }

        [JsonIgnore]
        public virtual BaseCreature Creature { get; set; }

        public override bool CanEquip(BaseCreature creature) {
            return true;
        }
        public bool Equip(BaseCreature creature)
        {
            return creature.WeaponSet.AddWeapon(this);
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

        public string GetActionsString() { 
            StringBuilder sb = new();
            foreach (BaseAction action in Actions)
            {
                sb.AppendLine($"-{action}");
            }
            return sb.ToString();
        }

        public override string GetTooltipText()
        {
            StringBuilder sb = new();
            sb.AppendLine($"<size=175%><b>{Name}</b></size>");
            sb.AppendLine(Description);
            sb.AppendLine($"<size=150%><b>Actions</b></size>");
            sb.AppendLine($"{GetActionsString()}");
            return sb.ToString();
        }
    }
}
