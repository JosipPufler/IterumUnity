using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IClass : IResistable
    {
        string ClassName { get; }
        int Level { get; set; }
        IList<IAction> Actions { get; }
        IDictionary<Attribute, int> AttributesModifiers { get; }
        IDictionary<Attribute, double> AttributesMultipiers { get; }
        IDictionary<DamageCategory, double> DamageCategoryResistances { get; }

        bool LevelUp();
        bool CanJoin(ICreature creature);
    }
}
