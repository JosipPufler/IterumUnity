using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
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

            ConsumeAction = new BaseAction() {
                Name = "Devour",
                Description = "Devour the flesh of a slain creature to heal yourself", 
                ApCost = 1,
                MpCost = 0,
                TargetTypes = new Dictionary<TargetData, int>() {
                {
                    new TargetData(TargetType.Creature, 0, 1), 1
                } }, 
                Action = Action
            };
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
