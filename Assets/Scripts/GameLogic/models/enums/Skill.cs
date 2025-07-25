// Assets/Scripts/Utils/Skill.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using Unity.VisualScripting;

namespace Iterum.models.enums
{
    [JsonConverter(typeof(SkillConverter))]
    public class Skill
    {
        private Skill(string name, Stat stat, Attribute attribute)
        {
            Name = name;
            Stat = stat;
            Attribute = attribute;
        }

        public string Name { get; set; }
        public Attribute Attribute { get; }
        public Stat Stat { get; set; }

        // Strength
        public static readonly Skill Athletics = new("Athletics", Stat.Strength, Attribute.Athletics);

        // Agility
        public static readonly Skill Acrobatics = new("Acrobatics", Stat.Agility, Attribute.Acrobatics);
        public static readonly Skill SlightOfHand = new("Slight of Hand", Stat.Agility, Attribute.SlightOfHand);
        public static readonly Skill Stealth = new("Stealth", Stat.Agility, Attribute.Stealth);

        // Endurance
        public static readonly Skill Perception = new("Perception", Stat.Endurance, Attribute.Perception);
        public static readonly Skill Grit = new("Grit", Stat.Endurance, Attribute.Grit);

        // Willpower
        public static readonly Skill Determination = new("Determination", Stat.Willpower, Attribute.Determination);
        public static readonly Skill Intuition = new("Intuition", Stat.Willpower, Attribute.Intuition);

        // Faith
        public static readonly Skill Religion = new("Religion", Stat.Faith, Attribute.Religion);
        public static readonly Skill Ritualism = new("Ritualism", Stat.Faith, Attribute.Ritualism);
        public static readonly Skill Occult = new("Occult", Stat.Faith, Attribute.Occult);
        public static readonly Skill Medicine = new("Medicine", Stat.Faith, Attribute.Medicine);

        // Intelligence
        public static readonly Skill Arcana = new("Arcana", Stat.Intelligence, Attribute.Arcana);
        public static readonly Skill History = new("History", Stat.Intelligence, Attribute.History);
        public static readonly Skill Nature = new("Nature", Stat.Intelligence, Attribute.Nature);
        public static readonly Skill Technology = new("Technology", Stat.Intelligence, Attribute.Technology);

        // Charisma
        public static readonly Skill Performance = new("Performance", Stat.Charisma, Attribute.Performance);
        public static readonly Skill Persuasion = new("Persuasion", Stat.Charisma, Attribute.Persuasion);
        public static readonly Skill Deception = new("Deception", Stat.Charisma, Attribute.Deception);
        public static readonly Skill StreetSmarts = new("Street Smarts", Stat.Charisma, Attribute.StreetSmarts);
        public static readonly Skill Barter = new("Barter", Stat.Charisma, Attribute.Barter);

        public static IEnumerable<Skill> GetAllSkills() => new List<Skill>(){
            Athletics,

            Acrobatics,
            SlightOfHand,
            Stealth,

            Perception,
            Grit,

            Determination,
            Intuition,

            Religion,
            Ritualism,
            Occult,
            Medicine,

            Arcana,
            History,
            Nature,
            Technology,

            Performance,
            Persuasion,
            Deception,
            StreetSmarts,
            Barter
        };

        public static Skill FromName(string name) =>
            GetAllSkills().FirstOrDefault(s => s.Name.Replace(" ", "").Equals(name.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Unknown skill name: {name}");

        public static Skill FromAttribute(Attribute attribute) =>
            GetAllSkills().FirstOrDefault(s => s.Attribute.Equals(attribute))
            ?? throw new ArgumentException($"Unknown skill attribute: {attribute}");
    }
}
