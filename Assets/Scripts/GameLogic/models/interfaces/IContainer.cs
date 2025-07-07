using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IContainer
    {
        IDictionary<IItem, int> Inventory { get; }
    }
}
