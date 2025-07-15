using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.Utils.converters;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Iterum.models
{
    public class ProficiencyManager
    {
        public ProficiencyManager(ICreature creature) {
            this.creature = creature;
        }

        public ProficiencyManager() { }

        [JsonIgnore]
        public ICreature creature;

        [JsonProperty]
        [JsonConverter(typeof(DictionaryKeySkillConverter))]
        public Dictionary<Skill, int> SkillProficiencies { get; private set; } = new();

        [JsonProperty]
        public HashSet<Stat> SavingThrowProficiencies { get; private set; } = new HashSet<Stat>();

        [JsonProperty]
        public HashSet<WeaponType> WeaponProficiencies { get; private set; } = new() {WeaponType.Natural};

        public int GetProficiencyBonus()
        {
            return (int)Math.Ceiling(creature.ClassManager.GetLevel() / 2.0);
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

        public bool IsProficient(IWeapon weapon) {
            return WeaponProficiencies.Contains(weapon.WeaponType);
        }
    }
}
