using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.Utils;
using Iterum.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System.Collections.Generic;
using System.Linq;

namespace Iterum.models.items
{
    public class CorpseRation : BaseConsumable
    {
        private static int HealAmmount = 10;

        public CorpseRation(BaseCreature creature) { 
            this.creature = creature;
            Name = $"{StaticUtils.GetName(creature.Race.GetType())} ration";

            ConsumeAction = new Consume(this, "Devour the flesh of a slain creature to heal yourself", 1, 0, new Dictionary<TargetData, int>() { 
                { 
                    new TargetData(TargetType.Creature, 0, 1), 1 
                } }, Action);
        }

        static ActionResult Action(ActionInfo actionInfo)
        {
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);

            BaseCreature targetable = ((TargetDataSubmissionCreature)actionInfo.Targets.FirstOrDefault(x => x.Key.TargetType == TargetType.Creature).Value.First()).GetToken().creature;
            targetable.Heal(HealAmmount);
            actionResultBuilder.AmountHeald(targetable, HealAmmount);
            return actionResultBuilder.Build();
        }

        private BaseCreature creature;

        public override double Weight { get; set; } = 2;

        public override bool Stackable { get; set; } = false;
    }
}
