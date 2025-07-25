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

        public static IDictionary<DamageType, double> CalculateEfectiveDamage(
    IEnumerable<IResistable> resistables,
    IDictionary<DamageType, double> additionalModifiers)
        {
            // 1) Compute category resistances
            IDictionary<DamageCategory, double> categoryResistances = CalculateEfectiveCategoryDamage(resistables);

            // 2) Gather all raw resistances (from each IResistable and any extra modifiers)
            var rawEntries = resistables
                .SelectMany(r => r.Resistances)
                .Concat(additionalModifiers);

            // Log each raw entry
            foreach (var entry in rawEntries)
            {
                Debug.Log($"Raw resistance: Type={entry.Key}, Value={entry.Value}");
            }

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
                            prod *= (1 - e.Value);
                        }
                        return prod;
                    });

            // Log after aggregation, before category adjustment
            foreach (var kv in resistances)
            {
                Debug.Log($"After aggregation: Type={kv.Key}, Multiplier={kv.Value}");
            }

            // 4) Apply category multipliers
            foreach (DamageType damageType in resistances.Keys.ToList())
            {
                if (categoryResistances.TryGetValue(damageType.DamageCategory, out double categoryMul))
                {
                    double before = resistances[damageType];
                    resistances[damageType] *= categoryMul;
                    Debug.Log(
                        $"Applying category {damageType.DamageCategory}: " +
                        $"before={before}, categoryMul={categoryMul}, after={resistances[damageType]}");
                }
            }

            return resistances;
        }

        public static IDictionary<DamageType, double> CalculateEfectiveDamage(IEnumerable<IResistable> resistables)
        {
            return CalculateEfectiveDamage(resistables, new Dictionary<DamageType, double>());
        }

        public static IDictionary<DamageCategory, double> CalculateEfectiveCategoryDamage(IEnumerable<IResistable> resistables, IDictionary<DamageCategory, double> additionalModifiers)
        {
            return resistables
                .SelectMany(resistable => resistable.CategoryResistances)
                .Concat(additionalModifiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product * (1 - entity.Value)));
        }

        public static IDictionary<DamageCategory, double> CalculateEfectiveCategoryDamage(IEnumerable<IResistable> resistables)
        {
            return CalculateEfectiveCategoryDamage(resistables, new Dictionary<DamageCategory, double>());
        }
    }
}
