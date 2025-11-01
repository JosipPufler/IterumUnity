using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.items;
using Iterum.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iterum.models.interfaces
{
    public interface IWeapon : ITargetable, IEquipment, ITooltipTarget
    {
        IList<DamageInfo> DamageInfos { get; }
        int ReachModifier { get; }
        IList<WeaponTrait> WeaponTraits { get; }
        WeaponType WeaponType { get; }
        WeaponSlotDetails WeaponSlotDetails { get; set; }
        BaseCreature Creature { get; set; }
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

        string GetDamageInfoString();
    }
}
