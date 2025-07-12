using Assets.Scripts.GameLogic.models.races;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Iterum.models.races
{
    public class Boring : BaseRace
    {
        public override string Name { get; set; } = "Race";
        public override string Description { get; set; } = "new Desc";

        public override Dictionary<WeaponSlot, int> WeaponSlots { get; } = new Dictionary<WeaponSlot, int>() { { WeaponSlot.Special, 1} };

        public override Dictionary<Attribute, int> RacialAttributes { get; } = new Dictionary<Attribute, int>() { { Attribute.MaxHp, 20 }, { Attribute.MaxAp, 6 } };
    }
}
