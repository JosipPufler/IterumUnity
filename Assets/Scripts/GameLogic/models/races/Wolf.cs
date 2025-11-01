using Assets.Scripts.GameLogic.models.races;
using Iterum.models.enums;
using System.Collections.Generic;

namespace Iterum.models.races
{
    public class Wolf : BaseRace
    {
        public override string Name { get; set; } = "Wolf";
        public override string Description { get; set; } = "Wolves are predators that roam the steppes and forests of this world. They excell at pack hunting but pose little threat to seasoned adventurers.";

        public override Dictionary<WeaponSlot, int> WeaponSlots { get; protected set; } = new Dictionary<WeaponSlot, int>() { { WeaponSlot.Special, 1} };

        public override Dictionary<Attribute, int> RacialAttributes { get; protected set; } = new Dictionary<Attribute, int>() { { Attribute.MaxHp, 20 } };
    }
}
