using Iterum.models;
using Iterum.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameLogic.models.items
{
    public class SmallHealtPotion : IConsumable
    {
        public SmallHealtPotion()
        {
            ConsumeAction = new Consume(this, "Heal yourself by 2d4 + 4 HP", 2, 0, new Dictionary<TargetData, int>() {
                {
                    new TargetData(TargetType.Creature, 0, 1), 1
                } }, Action);
        }

        public Consume ConsumeAction { get; }

        public double Weight { get; } = 1;

        public string Name { get; } = "Small health potion";

        static ActionResult Action(ActionInfo actionInfo)
        {
            ITargetable targetable = (ITargetable)actionInfo.Targets.FirstOrDefault(x => x.Key.TargetType == TargetType.Creature).Value.First().Targetable;
            if (targetable != null && targetable is ICreature targetCreature)
            {
                targetCreature.Heal(DiceUtils.RollMultiple(2, Dice.d4, RollType.Normal).Sum() + 4);
            }
            return null;
        }
    }
}
