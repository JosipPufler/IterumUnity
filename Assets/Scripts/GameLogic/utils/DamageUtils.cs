using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Iterum.utils
{
    public static class DamageUtils
    {
        public static IDictionary<DamageType, double> CalculateEfectiveDamage(IEnumerable<IResistable> resistables, IDictionary<DamageType, double> additionalModifiers)
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
