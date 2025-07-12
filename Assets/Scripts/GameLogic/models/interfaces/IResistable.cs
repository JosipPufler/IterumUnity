using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IResistable
    {
        Dictionary<DamageType, double> Resistances { get; }
        Dictionary<DamageCategory, double> CategoryResistances { get; }
    }
}
