using System;
using System.Collections.Generic;

namespace Iterum.models.enums
{
    public class DamageCategory
    {
        private DamageCategory(string name, DamageClass damageClass) {
            Name = name;
            DamageClass = damageClass;
        }

        public string Name { get; }
        public DamageClass DamageClass { get; }

        public static readonly DamageCategory Biological = new("Biological", DamageClass.Health);
        public static readonly DamageCategory True = new("True", DamageClass.Health);
        public static readonly DamageCategory Mental = new("Mental", DamageClass.Sanity);
        public static readonly DamageCategory Physical = new("Physical", DamageClass.Health);
        public static readonly DamageCategory Elemental = new("Elemental", DamageClass.Health);
        public static readonly DamageCategory Aetherial = new("Aetherial", DamageClass.Health);

        private static readonly Dictionary<string, DamageCategory> _byName =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { Biological.Name, Biological },
            { True.Name, True },
            { Mental.Name, Mental },
            { Physical.Name, Physical },
            { Elemental.Name, Elemental },
            { Aetherial.Name, Aetherial },
        };

        public static DamageCategory FromName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (_byName.TryGetValue(name, out var category))
                return category;

            throw new ArgumentException($"Unknown damage category: '{name}'", nameof(name));
        }
    }
}
