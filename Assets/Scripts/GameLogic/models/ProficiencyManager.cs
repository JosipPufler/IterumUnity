using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;

namespace Iterum.models
{
    public class ProficiencyManager
    {
        public ProficiencyManager(ICreature creature) { 
            this.creature = creature;
        }

        ICreature creature;
        Dictionary<Skill, int> SkillProficiencies = new Dictionary<Skill, int>();
        
        HashSet<Stat> SavingThrowProficiencies { get; } = new HashSet<Stat>();

        public int GetProficiencyBonus()
        {
            return (int)Math.Ceiling(creature.ClassManager.GetLevel() / 4.0);
        }

        public int GetSavingThrowProficiencyBonus(Stat stat)
        {
            if (SavingThrowProficiencies.Contains(stat))
            {
                return GetProficiencyBonus();
            }
            return 0;
        }

        public int GetSkillProficiencyBonus(Skill skill) 
        {
            if (SkillProficiencies.TryGetValue(skill, out int number))
            {
                if (number >= 2)
                    return GetProficiencyBonus() * 2;
                if (number == 1)
                    return GetProficiencyBonus();
                if (number <= 0)
                    SkillProficiencies.Remove(skill);
            }
            return 0;
        }

        public void AddSkillProficiency(Skill skill) 
        {
            if (!SkillProficiencies.ContainsKey(skill))
            {
                SkillProficiencies[skill] = 0;
            }
            SkillProficiencies[skill]++;
        }

        public void RemoveSkillProficiency(Skill skill)
        {
            if (SkillProficiencies.TryGetValue(skill, out int value))
            {
                SkillProficiencies[skill] = --value;
                if (value <= 0)
                {
                    SkillProficiencies.Remove(skill);
                }
            }
        }
    }
}
