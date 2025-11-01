using Assets.Scripts.Utils.converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Iterum.models.enums
{
    public class DamageType
    {
        public string Name { get; set; }
        [JsonConverter(typeof(DamageCategoryConverter))]
        public DamageCategory DamageCategory { get; set; }

        public static readonly DamageType Piercing = new("Piercing", DamageCategory.Physical);
        public static readonly DamageType Slashing = new("Slashing", DamageCategory.Physical);
        public static readonly DamageType Blunt = new("Blunt", DamageCategory.Physical);
        public static readonly DamageType True = new("True", DamageCategory.True);
        public static readonly DamageType Fire = new("Fire", DamageCategory.Elemental);
        public static readonly DamageType Ice = new("Ice", DamageCategory.Elemental);
        public static readonly DamageType Lightning = new("Lightning", DamageCategory.Elemental);
        public static readonly DamageType Infernal = new("Infernal", DamageCategory.Aetherial);
        public static readonly DamageType Necrotic = new("Necrotic", DamageCategory.Aetherial);
        public static readonly DamageType Divine = new("Divine", DamageCategory.Aetherial);
        public static readonly DamageType Acid = new("Acid", DamageCategory.Biological);
        public static readonly DamageType Poison = new("Poison", DamageCategory.Biological);
        public static readonly DamageType Maddening = new("Maddening", DamageCategory.Mental);

        public DamageType(string name, DamageCategory damageCategory)
        {
            Name = name;
            DamageCategory = damageCategory;
        }

        public static IList<DamageType> GetDamageTypes() {
            return new List<DamageType>() { 
                Piercing, Slashing, Blunt, True, Fire, Ice, Lightning, Infernal, Necrotic, Divine, Acid, Poison, Maddening, 
            };
        }

        public static DamageType FromName(string name)
        {
            return GetDamageTypes().FirstOrDefault(x => x.Name == name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
