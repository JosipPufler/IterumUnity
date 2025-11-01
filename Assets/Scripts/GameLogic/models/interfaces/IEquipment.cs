using Assets.Scripts.GameLogic.models.actions;
using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IEquipment : IItem
    {
        bool CanEquip(BaseCreature creature);

        Dictionary<Attribute, int> AttributeModifiers {  get; }
        Dictionary<Attribute, double> AttributeMultipliers { get; }

        List<BaseAction> Actions { get; }
    }
}
