using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IActionable
    {
        int CurrentAp { get; set; }
        int MaxAp { get; set; }
        int OriginalMaxAp { get; }
        int ApRegen { get; set; }
        int OriginalApRegen { get; }

        IList<IAction> GetActions();
    }
}
