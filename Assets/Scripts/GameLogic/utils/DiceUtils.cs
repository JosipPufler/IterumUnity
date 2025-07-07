using Iterum.models.enums;
using System;
using System.Collections.Generic;

namespace Iterum.utils
{
    internal static class DiceUtils
    {
        static Random random = new Random();

        public static IList<int> RollMultiple(int numberOfDice, Dice die, RollType rollType) { 
            IList<int> listOfRolls = new List<int>();
            for (int i = 0; i < numberOfDice; i++)
            {
                listOfRolls.Add(Roll(die, rollType));
            }
            return listOfRolls;
        }

        public static int Roll(Dice die, RollType rollType) {
            int roll = random.Next(0, (int)die + 1);
            
            switch (rollType)
            {
                case RollType.Advantage:
                    int rollWithAdvantage = random.Next(0, (int)die + 1);
                    if (rollWithAdvantage >= roll)
                    {
                        return rollWithAdvantage;
                    }
                    return roll;
                
                case RollType.Normal:
                    return roll;

                case RollType.Disadvantage:
                    int rollWithDisadvantage = random.Next(0, (int)die + 1);
                    if (rollWithDisadvantage <= roll)
                    {
                        return rollWithDisadvantage;
                    }
                    return roll;
                
                default:
                    throw new Exception("Unsupported roll type");
            }
        }
    }
}
