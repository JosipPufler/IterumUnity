using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IRace : IResistable
    {
        Dictionary<WeaponSlot, int> WeaponSlots { get; }
        Dictionary<Attribute, int> RacialAttributes { get; }
        Dictionary<Attribute, double> RacialAttributeMultipliers { get; }
        Dictionary<ArmorSlot, int> ArmorSlots { get; }

        IList<IAction> GetActions();

        IDictionary<DamageType, double> GetEffectiveDamageResistances();
            
        IDictionary<DamageCategory, double> GetEffectiveDamageCategoryResistances();
    }
}
