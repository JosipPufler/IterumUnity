using Assets.Scripts.GameLogic.models.interfaces;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Iterum.utils
{
    public static class DamageUtils
    {
        /*public static IDictionary<DamageType, double> CalculateEfectiveDamage(IEnumerable<IResistable> resistables, IDictionary<DamageType, double> additionalModifiers)
        {
            IDictionary<DamageCategory, double> categoryResistances = CalculateEfectiveCategoryDamage(resistables);
            Dictionary<DamageType, double> resistances = resistables
                .SelectMany(resistable => resistable.Resistances)
                .Concat(additionalModifiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product * (1 - entity.Value)));
            foreach (DamageType damageType in resistances.Keys)
            {
                if (categoryResistances.TryGetValue(damageType.DamageCategory, out double category))
                {
                    resistances[damageType] *= category;
                }
            }
            return resistances;
        }*/

        public static Dictionary<DamageType, double> CalculateEfectiveDamage(
            IEnumerable<IResistable> resistables,
            IDictionary<DamageType, double> additionalModifiers)
        {
            // 1) Compute category resistances
            IDictionary<DamageCategory, double> categoryResistances = CalculateEfectiveCategoryDamage(resistables);

            // 2) Gather all raw resistances (from each IResistable and any extra modifiers)
            IEnumerable<KeyValuePair<DamageType, double>> rawEntries = resistables
                .SelectMany(r => {
                    if (r is BaseArmor armor)
                    {
                        return r.Resistances.ToDictionary(x => x.Key, x => x.Value * armor.ArmorSlot.ArmorMultiplier);
                    }
                    return r.Resistances;
                })
                .Concat(additionalModifiers);

            // 3) Aggregate into a per-DamageType multiplier
            var resistances = rawEntries
                .GroupBy(e => e.Key)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        // multiply (1 - each resistance) together
                        double prod = 1.0;
                        foreach (var e in g)
                        {
                            prod -= e.Value;
                        }
                        return prod;
                    });

            foreach (var damageType in DamageType.GetDamageTypes())
            {
                if (!resistances.ContainsKey(damageType))
                    resistances[damageType] = 1.0;
            }

            // 4) Apply category multipliers
            foreach (DamageType damageType in resistances.Keys.ToList())
            {
                if (categoryResistances.TryGetValue(damageType.DamageCategory, out double categoryMul))
                {
                    double before = resistances[damageType];
                    resistances[damageType] *= categoryMul;
                }
            }

            return resistances;
        }

        public static Dictionary<DamageType, double> CalculateEfectiveDamageResistances(IEnumerable<IResistable> resistables)
        {
            return CalculateEfectiveDamage(resistables, new Dictionary<DamageType, double>());
        }

        public static IDictionary<DamageCategory, double> CalculateEfectiveCategoryDamage(IEnumerable<IResistable> resistables, IDictionary<DamageCategory, double> additionalModifiers)
        {
            return resistables
                .SelectMany(resistable => {
                    if (resistable is BaseArmor armor)
                    {
                        return resistable.CategoryResistances.ToDictionary(x => x.Key, x => x.Value*armor.ArmorSlot.ArmorMultiplier);
                    }
                    return resistable.CategoryResistances;
                    })
                .Concat(additionalModifiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product -= entity.Value));
        }

        public static IDictionary<DamageCategory, double> CalculateEfectiveCategoryDamage(IEnumerable<IResistable> resistables)
        {
            return CalculateEfectiveCategoryDamage(resistables, new Dictionary<DamageCategory, double>());
        }
    }
}
