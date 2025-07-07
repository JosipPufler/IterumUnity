using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Iterum.models.races
{
    public class Boring : IRace
    {
        public string Name { get; } = "Race";

        public IDictionary<WeaponSlot, int> WeaponSlots { get; } = new Dictionary<WeaponSlot, int>() { { WeaponSlot.Special, 1} };

        public IDictionary<Attribute, int> RacialAttributes { get; } = new Dictionary<Attribute, int>() { { Attribute.MaxHp, 20 }, { Attribute.MaxAp, 6 } };

        public IDictionary<Attribute, double> RacialAttributeMultipliers { get; } = new Dictionary<Attribute, double>();

        public IDictionary<DamageType, double> Resistances { get; } = new Dictionary<DamageType, double>();

        public IDictionary<DamageCategory, double> CategoryResistances { get; } = new Dictionary<DamageCategory, double>();

        public IList<IAction> GetActions()
        {
            return new List<IAction>();
        }
    }
}
