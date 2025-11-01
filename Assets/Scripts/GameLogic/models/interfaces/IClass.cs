using Assets.Scripts.GameLogic.models.actions;
using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IClass : IResistable
    {
        int Level { get; set; }
        Dictionary<Attribute, int> AttributesModifiers { get; }
        Dictionary<Attribute, double> AttributesMultipiers { get; }
        Dictionary<DamageCategory, double> DamageCategoryResistances { get; }

        bool LevelUp();
        bool CanJoin(BaseCreature creature);
    }
}
