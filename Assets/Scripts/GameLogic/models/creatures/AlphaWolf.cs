using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.models.items;
using Iterum.models.races;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.GameLogic.models.creatures
{
    public class AlphaWolf : BaseCreature
    {
        public new static string DisplayName { get; private set; } = "Alpha wolf";

        public AlphaWolf() : base(new Boring(), DisplayName, "Textures/alphawolf")
        {
            WeaponSet.AddWeapon(new WolfTeeth());
            Race.RacialAttributes[Attribute.MaxHp] = 40;
            CurrentHp = MaxHp;
            MaxAp = 8;
            ModifierManager.SetModifier(Attribute.Strength, 4);
        }
    }
}
