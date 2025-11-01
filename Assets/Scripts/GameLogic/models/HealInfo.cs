using Assets.Scripts.GameLogic.models.enums;
using Iterum.models.enums;
using Iterum.utils;
using System;
using System.Linq;

namespace Assets.Scripts.GameLogic.models
{
    public class HealInfo
    {
        public HealInfo(int numberOfDice, Dice die, HealType healType, bool halved = false)
        {
            Die = die;
            NumberOfDice = numberOfDice;
            HealType = healType;
            Halved = halved;
        }

        public Dice Die { get; set; }
        public int NumberOfDice { get; set; }
        public HealType HealType { get; set; }
        public bool Halved { get; set; }

        public HealResult GetResult(RollType rollType)
        {
            int result = DiceUtils.RollMultiple(NumberOfDice, Die, rollType).Sum();
            if (Halved)
            {
                result = (int)Math.Ceiling(result / 2f);
            }
            return new HealResult(result, HealType);
        }

        public override string ToString()
        {
            return $"{NumberOfDice}{Die} {HealType} healing";
        }
    }
}
