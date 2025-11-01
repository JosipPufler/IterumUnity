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

        [JsonProperty]
        public virtual Dictionary<WeaponSlot, int> WeaponSlots { get; protected set; } = new Dictionary<WeaponSlot, int>() { { WeaponSlot.Hand, 2 } };

        [JsonProperty]
        public virtual Dictionary<Attribute, int> RacialAttributes { get; protected set; } = new Dictionary<Attribute, int>();

        [JsonProperty]
        public virtual Dictionary<Attribute, double> RacialAttributeMultipliers { get; protected set; } = new Dictionary<Attribute, double>();

        [JsonProperty]
        public virtual Dictionary<DamageType, double> Resistances { get; protected set; } = new Dictionary<DamageType, double>();

        [JsonProperty]
        public virtual Dictionary<DamageCategory, double> CategoryResistances { get; protected set; } = new Dictionary<DamageCategory, double>();

        [JsonProperty]
        [JsonConverter(typeof(DictionaryKeyArmorSlotConverterInt))]
        public virtual Dictionary<ArmorSlot, int> ArmorSlots { get; protected set; } = new Dictionary<ArmorSlot, int>() {
                                                                                            { ArmorSlot.Head, 1 },
                                                                                            { ArmorSlot.Torso, 1 },
                                                                                            { ArmorSlot.Hand, 2 },
                                                                                            { ArmorSlot.Legs, 1 },
                                                                                            { ArmorSlot.Ring, 10 },
                                                                                            { ArmorSlot.Necklace, 1 },
                                                                                            { ArmorSlot.Boots, 1 },
                                                                                        };
        [JsonProperty]
        public virtual int SkillPointPicks { get; protected set; }
        [JsonProperty]
        public virtual HashSet<Skill> RacialSkills { get; protected set; } = new HashSet<Skill>();

        public virtual int BaseEvasionRating { get; set; } = 10;
        public virtual IList<IAction> GetActions()
        {
            return new List<IAction>();
        }

        public virtual IDictionary<DamageType, double> GetEffectiveDamageResistances()
        {
            return DamageUtils.CalculateEfectiveDamageResistances(new IResistable[] { this });
        }

        public virtual IDictionary<DamageCategory, double> GetEffectiveDamageCategoryResistances()
        {
            return DamageUtils.CalculateEfectiveCategoryDamage(new IResistable[] { this });
        }
    }
}
