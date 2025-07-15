using Assets.Scripts.GameLogic.models.items;
using Iterum.models.interfaces;
using Iterum.models.items;
using Iterum.models.races;
using UnityEngine;

namespace Iterum.models.creatures
{
    public class Wolf : ICreature
    {
        public new static string DisplayName { get; private set; } = "Wolf";

        public Wolf() : base(new Boring(), DisplayName, "Textures/wolf")
        {
            WeaponSet.AddWeapon(new WolfTeeth());
            Inventory.Add(new SmallHealtPotion(), 1);
            CurrentHp = MaxHp;
        }
    }
}
