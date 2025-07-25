using Assets.Scripts.GameLogic.models.interfaces;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IContainer
    {
        List<BaseItem> Inventory { get; }
    }
}
