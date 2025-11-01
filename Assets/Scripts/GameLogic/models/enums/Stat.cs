using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.converters;
using Newtonsoft.Json;

namespace Iterum.models.enums
{
    [JsonConverter(typeof(StatConverter))]
    public class Stat
    {
        private Stat(string name, string shotName, Attribute attribute)
        {
            Name = name;
            ShotName = shotName;
            Attribute = attribute;
        }

        private Stat(string shotName, Attribute attribute)
        {
            Name = attribute.ToString();
            ShotName = shotName;
            Attribute = attribute;
        }

        public string Name { get; set; }
        public string ShotName { get; set; }
        public Attribute Attribute { get; set; }

        public static readonly Stat Strength = new("STR", Attribute.Strength);
        public static readonly Stat Agility = new("AGI", Attribute.Agility);
        public static readonly Stat Endurance = new("END", Attribute.Endurance);
        public static readonly Stat Willpower = new("WIL", Attribute.Willpower);
        public static readonly Stat Faith = new("FAI", Attribute.Faith);
        public static readonly Stat Intelligence = new("INT", Attribute.Intelligence);
        public static readonly Stat Charisma = new("CHA", Attribute.Charisma);

        public static IEnumerable<Stat> GetAllStats() => new List<Stat>() { Strength, Agility, Endurance, Willpower, Faith, Intelligence, Charisma };

        public static Stat FromName(string name) =>
            GetAllStats().FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Unknown stat name: {name}");
    }

    public enum StatEnum 
    {
        Strength,
        Agility,
        Endurance,
        Willpower,
        Faith,
        Intelligence,
        Charisma,
    }
}
