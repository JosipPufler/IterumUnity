using Iterum.models.enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Iterum.utils
{
    public static class DiceUtils
    {
        static readonly Random random = new();

        private static int RollOnce(Dice die) => random.Next(1, (int)die + 1);

        public static int Roll(Dice die, RollType rollType = RollType.Normal) {
            int firstRoll = RollOnce(die);
            return rollType switch
            {
                RollType.Normal => firstRoll,
                RollType.Advantage => Math.Max(firstRoll, RollOnce(die)),
                RollType.Disadvantage => Math.Min(firstRoll, RollOnce(die)),
                _ => throw new Exception("Unsupported roll type"),
            };
        }

        public static IEnumerable<int> RollMultiple(int numberOfDice, Dice die, RollType rollType)
        {
            List<int> listOfRolls = new();
            for (int i = 0; i < numberOfDice; i++)
            {
                listOfRolls.Add(Roll(die, rollType));
            }
            return listOfRolls;
        }
    }
}
