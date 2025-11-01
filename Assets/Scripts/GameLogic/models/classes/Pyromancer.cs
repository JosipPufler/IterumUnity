using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.creatures;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.classes
{
    public class Pyromancer : BaseClass
    {
        public Pyromancer() { }

        public override string ClassName { get; } = "Pyromancer";
        public override string Description { get; set; } = "Pyromancers are magical adepts that specialize in fire based spells that deal large amounts of damage often in a large area indescriminate of alligence. Starts with a set of robes.";
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public override Dictionary<int, List<IAction>> ClassActions { get; protected set; } = new Dictionary<int, List<IAction>>() {
            {1, new List<IAction>(){new DelayedFireBlast()} }
        };

        public override bool InitCreature(BaseCreature creature)
        {
            if (!base.InitCreature(creature))
            {
                return false;
            }
            AttributesModifiers[Attribute.MaxMp] = 8 + creature.GetAttributeModifier(Attribute.Willpower);
            return true;
        }

        public override Dice HealthDie { get; set; } = Dice.d8;

        public override bool CanJoin(BaseCreature creature)
        {
            return creature.ModifierManager.GetAttribute(Attribute.Intelligence, true) >= 3;
        }
    }
}
