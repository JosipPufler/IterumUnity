using System.Collections.Generic;
using Assets.Scripts.GameLogic.models.creatures;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Unity.VisualScripting;

namespace Assets.Scripts.GameLogic.models.classes
{
    public class Warrior : BaseClass
    {
        public Warrior(){}

        public Warrior(ICreature creature) : base(creature)
        {
        }

        public override string ClassName { get; } = "Warrior";

        public override Dictionary<int, List<IAction>> ClassActions { get; } = new Dictionary<int, List<IAction>>();

        public override Dice HealthDie { get; set; } = Dice.d12;

        public override bool CanJoin(ICreature creature)
        {
            return creature.ModifierManager.GetAttribute(Attribute.Strength, false) >= 3;
        }

        public static new string Description { get; set; } = "Warrior is stong";
    }
}
