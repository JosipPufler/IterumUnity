using Assets.Scripts.GameLogic.models.interfaces;
using Iterum.models.enums;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.armor
{
    public class SteelArmor : ArmorGroup
    {
        public SteelArmor(ArmorSlot armorSlot) : base(armorSlot)
        {
            if (ArmorSlot == ArmorSlot.Torso || ArmorSlot == ArmorSlot.Legs)
            {
                EvasionRatingModifier = -1;
            }
        }

        public override double Weight { get; set; } = 1;
        public override string Name { get; set; } = "Steel armor";
        public override string Description { get; set; } = "Strong and heavy armor made of steel. Provides protection from physical attacks but conducts electricity";

        public override Dictionary<DamageType, double> Resistances { get; set; } = new Dictionary<DamageType, double>() { { DamageType.Lightning, -0.2 } };
        public override Dictionary<DamageCategory, double> CategoryResistances { get; set; } = new Dictionary<DamageCategory, double> { { DamageCategory.Physical, 0.5 } };
    }
}
