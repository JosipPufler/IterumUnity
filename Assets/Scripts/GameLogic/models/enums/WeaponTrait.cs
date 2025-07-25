using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.Utils.converters;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Iterum.models.enums
{
    [JsonConverter(typeof(WeaponTraitConverter))]
    public class WeaponTrait
    {
        public string Name { get; }
        public IList<BaseAction> Actions { get; }

        private WeaponTrait(string name, IList<BaseAction> actions) {
            Name = name;
            Actions = actions;
        }

        public static readonly WeaponTrait Reach = new("Reach", new List<BaseAction>());
        public static readonly WeaponTrait Light = new ("Light", new List<BaseAction>());
        public static readonly WeaponTrait Versatile = new ("Versatile", new List<BaseAction>());
        public static readonly WeaponTrait Heavy = new("Heavy", new List<BaseAction>());
        public static readonly WeaponTrait Finnes = new("Finnes", new List<BaseAction>());
        public static readonly WeaponTrait Natural = new("Natural", new List<BaseAction>());

        public static WeaponTrait ForName(string name)
        {
            return name switch
            {
                "Reach" => Reach,
                "Light" => Light,
                "Versatile" => Versatile,
                "Heavy" => Heavy,
                "Finnes" => Finnes,
                "Natural" => Natural,
                _ => throw new JsonSerializationException($"Unknown WeaponTrait '{name}'")
            };
        }
    }
}
