using Iterum.models.items;
using Iterum.models.races;
using Iterum.models;
using UnityEngine;
using Iterum.models.interfaces;
using Iterum.models.enums;

namespace Assets.Scripts.GameLogic.models.creatures
{
    public class AlphaWolf : ICreature
    {
        public new static string DisplayName { get; private set; } = "Alpha wolf";

        public AlphaWolf() : base(new Boring(), DisplayName, new Vector3Int(), "Textures/alphawolf")
        {
            WeaponSet.AddWeapon(new WolfTeeth());
            Race.RacialAttributes[Attribute.MaxHp] = 40;
            CurrentHp = MaxHp;
            MaxAp = 8;
            ModifierManager.SetModifier(Attribute.Strength, 4);
        }
    }
}
