using Assets.Scripts.GameLogic.armor;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.creatures;
using Assets.Scripts.GameLogic.models.items.weapons;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.classes
{
    public class Warrior : BaseClass
    {
        public Warrior(){}

        public override string ClassName { get; } = "Warrior";
        [JsonProperty]
        public override Dictionary<int, List<IAction>> ClassActions { get; protected set; } = new Dictionary<int, List<IAction>>() {
            {1, new List<IAction>(){new WhirlWind()} }
        };

        public override Dice HealthDie { get; set; } = Dice.d12;

        public override bool InitCreature(BaseCreature creature)
        {
            if (base.InitCreature(creature))
            {
                if (creature.ClassManager.GetLevel() == 0)
                {
                    creature.ArmorSet.CreateArmorSet<SteelArmor>(creature.GetArmorSlotsWithoutAccessories());
                    creature.WeaponSet.AddWeapon(new Greataxe(true));
                }
                return true;
            }
            return false;
        }

        public override bool CanJoin(BaseCreature creature)
        {
            return creature.ModifierManager.GetAttribute(Attribute.Strength, false) >= 3;
        }

        public override string Description { get; set; } = "Warriors are strong frontline combatants with the largest health die (d12) among all player classes. Warriors primarily use strength and large weapons to overwhelm enemies along with their large health pool to soak up enemy attacks. Starts with a greataxe and a set of steel armor.";
    }
}
