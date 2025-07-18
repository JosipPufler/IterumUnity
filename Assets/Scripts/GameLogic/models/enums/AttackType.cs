﻿using Assets.Scripts.Utils.converters;
using Iterum.models.enums;
using Newtonsoft.Json;

namespace Assets.Scripts.GameLogic.models.enums
{
    public class AttackType
    {
        public string Name { get; set; }
        public bool Proficient { get; set; }
        [JsonConverter(typeof(StatConverter))]
        public Stat BaseAttribute { get; set; }
        public AttackTypeEnum AttackTypeEnum { get; set; }
        public Attribute AttackTypeAttribute { get; set; }
        public Attribute AttackTypeDamageAttribute { get; set; }

        public AttackType(string name, Stat baseAttribute, bool proficient, AttackTypeEnum attackTypeEnum, Attribute attackTypeAttribute, Attribute attackTypeDamageAttribute)
        {
            Name = name;
            Proficient = proficient;
            AttackTypeEnum = attackTypeEnum;
            BaseAttribute = baseAttribute;
            AttackTypeAttribute = attackTypeAttribute;
            AttackTypeDamageAttribute = attackTypeDamageAttribute;
        }

        public static AttackType MeleeWeapon(Stat baseStat, bool proficient) {
            return new AttackType("Melee weapon attack", baseStat, proficient, AttackTypeEnum.MeleeWeapon, Attribute.MeleeWeaponAttackModifier, Attribute.MeleeWeaponDamageModifier);
        }

        public static AttackType RangedWeapon(Stat baseStat, bool proficient)
        {
            return new AttackType("Ranged weapon attack", baseStat, proficient, AttackTypeEnum.RangedWeapon, Attribute.RangedWeaponAttackModifier, Attribute.RangedWeaponDamageModifier);
        }

        public static AttackType SpellAttack(Stat baseStat, bool proficient)
        {
            return new AttackType("Spell attack", baseStat, proficient, AttackTypeEnum.Spell, Attribute.SpellAttackModifier, Attribute.SpellDamageModifier);
        }

        public static AttackType CreateByEnum(AttackTypeEnum attackType, Stat stat, bool proficient) {
            return attackType switch
            {
                AttackTypeEnum.RangedWeapon => RangedWeapon(stat, proficient),
                AttackTypeEnum.Spell => SpellAttack(stat, proficient),
                AttackTypeEnum.MeleeWeapon => MeleeWeapon(stat, proficient),
                _ => null,
            };
        }
    }

    public enum AttackTypeEnum { 
        RangedWeapon,
        Spell, 
        MeleeWeapon,
    }
}
