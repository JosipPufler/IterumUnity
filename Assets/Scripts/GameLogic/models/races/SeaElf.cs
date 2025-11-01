using Iterum.models.enums;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.races
{
    public class SeaElf : BaseRace
    {
        public override string Name { get; set; } = "Sea elf";
        public override string Description { get; set; } = "Sea elves are a semi-aquatic subspecies of elves who dwell primarily in their sunken cities. They worship ancient sea gods who granted them the ability to breathe underwater, saving them from extinction.";

        public override Dictionary<Attribute, int> RacialAttributes { get; protected set; } = new Dictionary<Attribute, int>() { { Attribute.SwimmingSpeed, 2 } };

        public override HashSet<Skill> RacialSkills
        {
            get
            {
                return new HashSet<Skill>() { Skill.Religion, Skill.Ritualism };
            }
        }
    }
}
