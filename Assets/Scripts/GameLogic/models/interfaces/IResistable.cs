using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IResistable
    {
        IDictionary<DamageType, double> Resistances { get; }
        IDictionary<DamageCategory, double> CategoryResistances { get; }
    }
}
