using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IClass : IResistable
    {
        string Description { get; }
        int Level { get; set; }
        IList<IAction> Actions { get; }
        Dictionary<Attribute, int> AttributesModifiers { get; }
        Dictionary<Attribute, double> AttributesMultipiers { get; }
        Dictionary<DamageCategory, double> DamageCategoryResistances { get; }

        bool LevelUp();
        bool CanJoin(BaseCreature creature);
    }
}
