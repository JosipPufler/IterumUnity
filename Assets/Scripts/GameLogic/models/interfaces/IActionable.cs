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

        int CurrentMp { get; set; }
        int MaxMp { get; set; }
        int OriginalMaxMp { get; }
        void RegenMana(int mp)
        {
            if (mp > MaxMp)
            {
                CurrentMp = MaxMp;
            }
            else
            {
                CurrentMp = mp;
            }
        }

        IList<IAction> GetActions();
    }
}
