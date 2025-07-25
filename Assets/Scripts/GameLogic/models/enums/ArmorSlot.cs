using System;
using Newtonsoft.Json;

namespace Iterum.models.enums
{
    [JsonConverter(typeof(ArmorSlotConverter))]
    public struct ArmorSlot
    {
        public string Name { get; set; }
        public double ArmorMultiplier { get; set; }

        public static readonly ArmorSlot Head = new("Head", 0.15);
        public static readonly ArmorSlot Necklace = new("Necklace", 1);
        public static readonly ArmorSlot Torso = new("Torso", 0.3);
        public static readonly ArmorSlot Hand = new("Hand", 0.2);
        public static readonly ArmorSlot Ring = new("Ring", 1);
        public static readonly ArmorSlot Legs = new("Legs", 0.2);
        public static readonly ArmorSlot Boots = new("Boots", 0.1);

        public ArmorSlot(string name, double armorMultiplier)
        {
            Name = name;
            ArmorMultiplier = armorMultiplier;
        }

        public static ArmorSlot FromName(string name) => name switch
        {
            "Head" => Head,
            "Necklace" => Necklace,
            "Torso" => Torso,
            "Hand" => Hand,
            "Ring" => Ring,
            "Legs" => Legs,
            "Boots" => Boots,
            _ => throw new ArgumentException($"Unknown ArmorSlot name: {name}")
        };

        public override string ToString() => Name;

        public override bool Equals(object obj) => obj is ArmorSlot other && Equals(other);

        public bool Equals(ArmorSlot other) => Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(ArmorSlot left, ArmorSlot right) => left.Equals(right);
        public static bool operator !=(ArmorSlot left, ArmorSlot right) => !(left == right);
    }
}
