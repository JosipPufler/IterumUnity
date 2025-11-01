using Assets.Scripts.GameLogic.models.enums;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;
using static Iterum.models.interfaces.BaseCreature;
using Attribute = Iterum.models.enums.Attribute;

namespace Assets.Scripts.GameLogic.models.races
{
    public class Ork : BaseRace
    {
        public override string Name { get; set; } = "Ork";
        public override string Description { get; set; } = "Orks are a primitive invasive species that arrived with the human colonists. Orks are renovned for being tough and strong.";

        public override Dictionary<Attribute, int> RacialAttributes { get; protected set; } = new Dictionary<Attribute, int>() { { Attribute.Athletics, 1 }, { Attribute.Endurance, 1 } };

        public bool HardToKillUsed { get; private set; } = false;

        public override HashSet<Skill> RacialSkills { get; protected set; } = new HashSet<Skill>() { Skill.Grit, Skill.Athletics };

    public void RegisterPassives(BaseCreature creature) {
            creature.Register<LongRestData>(RegainHardToKill);
            creature.Register<DeathData>(HardToKill);
            creature.getPassiveAttributes += TheBiggerIAmTheBiggerIAm;
        }

        public void RegainHardToKill(LongRestData longRestData) { 
            HardToKillUsed = false;
        }

        public void HardToKill(DeathData deathData) { 
            deathData.Creature.CurrentHp = 1;
            HardToKillUsed = true;
        }

        public Dictionary<Attribute, int> TheBiggerIAmTheBiggerIAm(GetAttributeData getAttributeData) {
            return new Dictionary<Attribute, int>() { { Attribute.Strength, (int)Math.Floor(getAttributeData.Creature.ClassManager.GetLevel() / 4.0) } };
        }
    }
}
