using Assets.Scripts.GameLogic.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.interfaces
{
    public class BaseArmor : BaseEquipment, IArmor
    {
        public virtual ArmorSlot ArmorSlot { get; set; }

        public virtual int EvasionRatingModifier
        {
            get => 0;
        }

        public virtual Dictionary<DamageType, double> Resistances { get; set; } = new();

        public virtual Dictionary<DamageCategory, double> CategoryResistances { get; set; } = new();
    }
}
