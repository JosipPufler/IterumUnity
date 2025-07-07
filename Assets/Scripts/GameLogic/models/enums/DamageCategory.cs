using System.Security.AccessControl;

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
    }
}
