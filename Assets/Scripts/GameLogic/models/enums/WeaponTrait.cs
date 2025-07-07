using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Iterum.models.enums
{
    public class WeaponTrait
    {
        public string Name { get; }
        public IList<IAction> Actions { get; }

        private WeaponTrait(string name, IList<IAction> actions) {
            Name = name;
            Actions = actions;
        }

        public static readonly WeaponTrait Reach = new("Reach", new List<IAction>());
        public static readonly WeaponTrait Light = new ("Light", new List<IAction>());
        public static readonly WeaponTrait Versatile = new ("Versatile", new List<IAction>());
        public static readonly WeaponTrait Heavy = new("Heavy", new List<IAction>());
        public static readonly WeaponTrait Finnes = new("Finnes", new List<IAction>());
    }
}
