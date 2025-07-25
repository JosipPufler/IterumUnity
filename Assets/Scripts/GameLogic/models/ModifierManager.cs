using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.Utils;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models
{
    public class ModifierManager
    {
        public Dictionary<Attribute, int> BaseAttributes { get; } = new() { { Attribute.ApRegen, 4 } };
        public Dictionary<Attribute, int> Modifiers { get; } = new();
        public Dictionary<DamageType, int> DamageTypeModifiers { get; } = new();
        public Dictionary<DamageCategory, int> DamageCategoryModifiers { get; } = new();

        public Dictionary<AttackTypeEnum, RollType> OutgoingAttackRolls { get; } = new() {
            { AttackTypeEnum.Spell, RollType.Normal },
            { AttackTypeEnum.MeleeWeapon, RollType.Normal },
            { AttackTypeEnum.RangedWeapon, RollType.Normal },
        };
        public Dictionary<AttackTypeEnum, RollType> IndboundAttackRolls { get; } = new() { 
            { AttackTypeEnum.Spell, RollType.Normal },
            { AttackTypeEnum.MeleeWeapon, RollType.Normal },
            { AttackTypeEnum.RangedWeapon, RollType.Normal },
        }; 

        public Dictionary<Attribute, double> Multipliers { get; } = new();
        public Dictionary<DamageType, double> Resistances { get; } = new();
        public Dictionary<DamageCategory, double> CategoryResistances { get; } = new();
        public Dictionary<WeaponSlot, int> WeaponSlots { get; } = new();

        [JsonConverter(typeof(DictionaryKeyArmorSlotConverterInt))]
        public Dictionary<ArmorSlot, int> ArmorSlots { get; } = new();

        [JsonIgnore]
        public BaseCreature creature;

        public ModifierManager() : this(null){}

        public ModifierManager(BaseCreature creature)
        {
            this.creature = creature;
        }

        public int GetAttribute(Attribute attribute, bool originalValue = false) {
            double total = 0;
            if (Modifiers.TryGetValue(attribute, out int modifier) && !originalValue)
            {
                total += modifier;
            }
            if (BaseAttributes.TryGetValue(attribute, out int baseAtt))
            {
                total += baseAtt;
            }
            if (creature.Race.RacialAttributes.TryGetValue(attribute, out int racial))
            {
                total += racial;
            }
            if (creature.ClassManager.GetAttributeModifiers().TryGetValue(attribute, out int classModifier))
            {
                total += classModifier;
            }
            if (creature.WeaponSet.GetAttributeModifiers().TryGetValue(attribute, out int weaponModifier))
            {
                total += weaponModifier;
            }
            if (creature.ArmorSet.GetAttributeModifiers().TryGetValue(attribute, out int armorModifier))
            {
                total += armorModifier;
            }

            if (Multipliers.TryGetValue(attribute, out double multiplier) && !originalValue)
            {
                total *= multiplier;
            }
            if (creature.ClassManager.GetAttributeMultipliers().TryGetValue(attribute, out double classMultiplier))
            {
                total *= classMultiplier;
            }
            if (creature.Race.RacialAttributeMultipliers.TryGetValue(attribute, out double raceMultiplier))
            {
                total *= raceMultiplier;
            }
            if (creature.WeaponSet.GetAttributeMultipliers().TryGetValue(attribute, out double weaponMultiplier))
            {
                total *= weaponMultiplier;
            }
            if (creature.ArmorSet.GetAttributeMultipliers().TryGetValue(attribute, out double armorMultiplier))
            {
                total *= armorMultiplier;
            }
            return (int)Math.Ceiling(total);
        }

        public double GetResistanceMultiplier(DamageType damageType, bool original)
        {
            double total = 1;
            if (!original)
            {
                total *= CalculateResistance(Resistances, damageType);
                total *= CalculateResistance(CategoryResistances, damageType.DamageCategory);
            }
            total *= CalculateResistance(creature.Race.GetEffectiveDamageResistances(), damageType);
            total *= CalculateResistance(creature.ClassManager.GetEffectiveDamageResistances(), damageType);
            total *= CalculateResistance(creature.Race.GetEffectiveDamageCategoryResistances(), damageType.DamageCategory);
            total *= CalculateResistance(creature.ClassManager.GetEffectiveDamageCategoryResistances(), damageType.DamageCategory);

            return total;
        }

        private static double CalculateResistance<T>(IDictionary<T, double> source, T key)
        {
            double total = 1;
            if (source.TryGetValue(key, out double universalDamageResistance))
            {
                total *= universalDamageResistance;
            }

            return total;
        }

        public void SetModifier(Attribute attribute, int modifier)
        {
            Modifiers[attribute] = Modifiers.GetValueOrDefault(attribute) + modifier;
        }

        public void ApplyModifiers(IDictionary<Attribute, int> modifiers) {
            foreach (Attribute attribute in modifiers.Keys)
            {
                Modifiers[attribute] = Modifiers.GetValueOrDefault(attribute) + modifiers[attribute];
            }
        }

        public void SetMultiplier(Attribute attribute, double multiplier)
        {
            Multipliers[attribute] = Multipliers.GetValueOrDefault(attribute) + multiplier;
        }

        public void SetResistance(DamageType damageType, double resistance)
        {
            Resistances[damageType] = Resistances.GetValueOrDefault(damageType) + resistance;
        }

        public int GetAttackModifier(AttackType attackType)
        {
            return GetAttribute(attackType.BaseAttribute.Attribute) 
                + GetAttribute(attackType.AttackTypeAttribute) 
                + (attackType.Proficient ? creature.ProficiencyManager.GetProficiencyBonus() : 0);
        }

        public int GetAttackDamageModifier(AttackType attackType) {
            return GetAttribute(attackType.BaseAttribute.Attribute) + GetAttribute(attackType.AttackTypeDamageAttribute);
        }

        public int GetDamageModifierForDamageType(DamageType damageType) {
            return DamageCategoryModifiers.GetValueOrDefault(damageType.DamageCategory) + DamageTypeModifiers.GetValueOrDefault(damageType);
        }
    }
}
