using Assets.Scripts.Utils.converters;
using Iterum.models.enums;
using Iterum.utils;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Iterum.models
{
    public class DamageInfo
    {
        public DamageInfo(int numberOfDice, Dice die, DamageType damageType, bool halved = false)
        {
            Die = die;
            NumberOfDice = numberOfDice;
            DamageType = damageType;
            Halved = halved;
        }

        public Dice Die { get; set; }
        public int NumberOfDice { get; set; }
        [JsonConverter(typeof(DamageTypeConverter))]
        public DamageType DamageType { get; set; }
        public bool Halved { get; set; }
    
        public DamageResult GetResult(RollType rollType)
        {
            int result = DiceUtils.RollMultiple(NumberOfDice, Die, rollType).Sum();
            if (Halved)
            {
                result = (int)Math.Ceiling(result / 2f);
            }
            return new DamageResult(result, DamageType);
        }

        public override string ToString()
        {
            return $"{NumberOfDice}{Die} {DamageType.Name} damage";
        }
    }
}
