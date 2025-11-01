using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.Utils.converters;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using IAction = Iterum.models.interfaces.IAction;

namespace Iterum.models.enums
{
    [JsonConverter(typeof(WeaponTraitConverter))]
    public class WeaponTrait
    {
        public string Name { get; }
        public IList<IAction> Actions { get; }
        public string Description { get; }

        private WeaponTrait(string name, IList<IAction> actions, string description = null)
        {
            Name = name;
            Actions = actions;
            Description = description;
        }

        public static readonly WeaponTrait Reach = new("Reach", new List<IAction>(), "These weapons are longer than usual providing greater reach when attacking.");
        public static readonly WeaponTrait Light = new ("Light", new List<IAction>(), "A lighter weapon allowing faster and easier attacks.");
        public static readonly WeaponTrait Heavy = new("Heavy", new List<IAction>(), "These weapons are cumbersome and require a certain level of strength just to weild as well as two hands.");
        public static readonly WeaponTrait Finesse = new("Finesse", new List<IAction>(), "These weapons require skill as well as strength to weild properly. Uses agility instead of strength when determining damage and attacks if agility is greater than strength.");
        public static readonly WeaponTrait Natural = new("Natural", new List<IAction>(), "These weapons are biological and a part of the creature itself.");

        public static WeaponTrait ForName(string name)
        {
            return name switch
            {
                "Reach" => Reach,
                "Light" => Light,
                //"Versatile" => Versatile,
                "Heavy" => Heavy,
                "Finesse" => Finesse,
                "Natural" => Natural,
                _ => throw new JsonSerializationException($"Unknown WeaponTrait '{name}'")
            };
        }

        public static List<WeaponTrait> GetAll() {
            return new List<WeaponTrait>() { Reach, Light, /*Versatile,*/ Heavy, Finesse, Natural };
        }
    }
}
