using Iterum.models.enums;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.races
{
    public class Human : BaseRace
    {
        public override string Name { get; set; } = "Human";
        public override string Description { get; set; } = "Humans are recent arrivals to this world and, as such, face many challenges. Though they lack innate magical abilities, their greatest strength lies in their versatility allowing them to master a wider range of skills than most races.";
        public override int SkillPointPicks { get; protected set; } = 2;

    }
}
