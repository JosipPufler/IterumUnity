using System.Collections.Generic;
using Assets.Scripts.Utils;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using Newtonsoft.Json;

namespace Assets.Scripts.GameLogic.models.races
{
    public class BaseRace : IRace
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual Dictionary<WeaponSlot, int> WeaponSlots { get; } = new Dictionary<WeaponSlot, int>();

        public virtual Dictionary<Attribute, int> RacialAttributes { get; } = new Dictionary<Attribute, int>();

        public virtual Dictionary<Attribute, double> RacialAttributeMultipliers { get; } = new Dictionary<Attribute, double>();

        public virtual Dictionary<DamageType, double> Resistances { get; } = new Dictionary<DamageType, double>();

        public virtual Dictionary<DamageCategory, double> CategoryResistances { get; } = new Dictionary<DamageCategory, double>();

        [JsonConverter(typeof(DictionaryKeyArmorSlotConverterInt))]
        public virtual Dictionary<ArmorSlot, int> ArmorSlots
        {
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

        public virtual IList<IAction> GetActions()
        {
            return new List<IAction>();
        }

        public virtual IDictionary<DamageType, double> GetEffectiveDamageResistances()
        {
            return DamageUtils.CalculateEfectiveDamage(new IResistable[] { this });
        }

        public virtual IDictionary<DamageCategory, double> GetEffectiveDamageCategoryResistances()
        {
            return DamageUtils.CalculateEfectiveCategoryDamage(new IResistable[] { this });
        }
    }
}
