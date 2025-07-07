using Iterum.models.enums;
using Iterum.utils;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IRace : IResistable
    {
        string Name { get; }
        IDictionary<WeaponSlot, int> WeaponSlots { get; }
        IDictionary<Attribute, int> RacialAttributes { get; }
        IDictionary<Attribute, double> RacialAttributeMultipliers { get; }
        IDictionary<ArmorSlot, int> ArmorSlots {
            get
            { 
                return new Dictionary<ArmorSlot, int>() {
                    { ArmorSlot.Head, 1 },
                    { ArmorSlot.Torso, 1 },
                    { ArmorSlot.Hand, 1 },
                    { ArmorSlot.Legs, 1 },
                    { ArmorSlot.Ring, 10 },
                    { ArmorSlot.Necklace, 1 },
                    { ArmorSlot.Boots, 1 },
                };
            } 
        }

        IList<IAction> GetActions();

        IDictionary<DamageType, double> GetEffectiveDamageResistances() 
        {
            return DamageUtils.CalculateEfectiveDamage(new IResistable[] { this });
        }

        IDictionary<DamageCategory, double> GetEffectiveDamageCategoryResistances()
        {
            return DamageUtils.CalculateEfectiveCategoryDamage(new IResistable[] { this });
        }
    }
}
