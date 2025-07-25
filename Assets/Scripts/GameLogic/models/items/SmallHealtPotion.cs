using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.target;
using Iterum.models;
using Iterum.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Assets.Scripts.GameLogic.models.items
{
    public class SmallHealtPotion : BaseConsumable
    {
        public SmallHealtPotion()
        {
            ConsumeAction = new Consume(this, "Heal yourself by 2d4 + 4 HP", 2, 0, new Dictionary<TargetData, int>() {
                {
                    new TargetData(TargetType.Creature, 0, 1), 1
                } }, Action);
        }

        public override void Initialize() {
            ConsumeAction.Action = Action;
        }

        public override double Weight { get; set; } = 1;

        public override string Name { get; set; } = "Small health potion";

        public override bool Stackable { get; set; } = true;

        static ActionResult Action(ActionInfo actionInfo)
        {
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);

            BaseCreature targetCreature = ((TargetDataSubmissionCreature)actionInfo.Targets.FirstOrDefault(x => x.Key.TargetType == TargetType.Creature).Value.First()).GetToken().creature;
            int ammountHealed = DiceUtils.RollMultiple(2, Dice.d4, RollType.Normal).Sum() + 4;
            targetCreature.Heal(ammountHealed);
            actionResultBuilder.AmountHeald(targetCreature, ammountHealed);
            return null;
        }
    }
}
