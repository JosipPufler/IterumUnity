using Assets.Scripts.Utils;
using Assets.Scripts.Utils.converters;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.interfaces
{
    public class BaseArmor : BaseEquipment, IArmor
    {
        public virtual ArmorSlot ArmorSlot { get; set; }

        public virtual int EvasionRatingModifier { get; set; } = 0;
        [JsonConverter(typeof(DictionaryKeyDamageTypeConverter))]
        public virtual Dictionary<DamageType, double> Resistances { get; set; } = new();

        public virtual Dictionary<DamageCategory, double> CategoryResistances { get; set; } = new();

        public bool Equip(BaseCreature creature) { 
            return creature.ArmorSet.AddArmor(this);
        }
    }
}
