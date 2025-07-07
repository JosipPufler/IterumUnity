using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IEquipment : IItem
    {
        bool CanEquip(ICreature creature);

        Dictionary<Attribute, int> AttributeModifiers {  get; }
        Dictionary<Attribute, double> AttributeMultipliers { get; }

        IList<IAction> Actions { get; }
    }
}
