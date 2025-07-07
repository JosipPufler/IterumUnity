using Iterum.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iterum.models.items
{
    public class CorpseRation : IConsumable
    {
        public CorpseRation(ICreature creature) { 
            this.creature = creature;
            Name = $"{creature.Race.Name} ration";

            ConsumeAction = new Consume(this, "Devour the flesh of a slain creature to heal yourself", 1, 0, new Dictionary<TargetType, int>() { { TargetType.Creature, 1 } }, Action);
        }

        static ActionResult Action(ActionInfo actionInfo)
        {
            ITargetable targetable = actionInfo.Targets.FirstOrDefault(x => x.TargetType == TargetType.Creature);
            if (targetable != null && targetable is ICreature targetCreature)
            {
                targetCreature.Heal(10);
            }
            return null;
        }

        private ICreature creature;

        public double Weight { get; } = 2;

        public string Name { get; }

        public Consume ConsumeAction { get; }
    }
}
