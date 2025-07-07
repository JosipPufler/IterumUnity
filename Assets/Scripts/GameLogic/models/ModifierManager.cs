using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models
{
    public class ModifierManager
    {
        IDictionary<Attribute, int> Modifiers { get; } = new Dictionary<Attribute, int>();
        IDictionary<Attribute, double> Multipliers { get; } = new Dictionary<Attribute, double>();
        IDictionary<DamageType, double> Resistances { get; } = new Dictionary<DamageType, double>();
        IDictionary<DamageCategory, double> CategoryResistances { get; } = new Dictionary<DamageCategory, double>();
        public IDictionary<WeaponSlot, int> WeaponSlots { get; } = new Dictionary<WeaponSlot, int>();
        public IDictionary<ArmorSlot, int> ArmorSlots { get; } = new Dictionary<ArmorSlot, int>();

        private readonly ICreature creature;

        public ModifierManager(ICreature creature)
        {
            this.creature = creature;
        }

        public int GetAttribute(Attribute attribute, bool original) {
            double total = 0;
            if (Modifiers.TryGetValue(attribute, out int modifier) && !original)
            {
                total += modifier;
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

            if (Multipliers.TryGetValue(attribute, out double multiplier) && !original)
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
            Modifiers[attribute] = modifier;
        }

        public void SetMultiplier(Attribute attribute, double multiplier)
        {
            Multipliers[attribute] = multiplier;
        }

        public void SetResistance(DamageType damageType, double resistance)
        {
            Resistances[damageType] = resistance;
        }
    }
}
