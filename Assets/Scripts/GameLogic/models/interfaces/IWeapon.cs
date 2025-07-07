using Iterum.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iterum.models.interfaces
{
    public interface IWeapon : ITargetable, IEquipment
    {
        IList<DamageInfo> DamageInfos { get; }
        int ReachModifier { get; }
        IList<WeaponTrait> WeaponTraits { get; }
        IList<IAction> WeaponActions { get; }
        WeaponSlotDetails WeaponSlotDetails { get; set; }

        int EvasionRatingBonus 
        {
            get => 0;
        }

        TargetType ITargetable.TargetType
        {
            get
            {
                return TargetType.Weapon;
            } 
        }

        IList<IAction> IEquipment.Actions =>
            WeaponActions
                .Concat(WeaponTraits.SelectMany(trait => trait.Actions))
                .ToList();

        string GetDamageInfoString() {
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
