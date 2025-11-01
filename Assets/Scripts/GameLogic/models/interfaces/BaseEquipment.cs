using Assets.Scripts.GameLogic.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.interfaces
{
    public class BaseEquipment : BaseItem, IEquipment
    {
        public override bool Stackable { get; set; } = false;

        public virtual Dictionary<Attribute, int> AttributeModifiers { get; set; } = new();

        public virtual Dictionary<Attribute, double> AttributeMultipliers { get; set; } = new();

        public virtual List<BaseAction> Actions { get; set; } = new List<BaseAction>();

        public virtual bool CanEquip(BaseCreature creature)
        {
            return true;
        }
    }
}
