using Assets.Scripts.Utils.converters;
using Iterum.models.enums;
using Iterum.utils;
using Newtonsoft.Json;
using System.Linq;

namespace Iterum.models
{
    public class DamageInfo
    {
        public DamageInfo(int numberOfDice, Dice die, DamageType damageType)
        {
            Die = die;
            NumberOfDice = numberOfDice;
            DamageType = damageType;
        }

        public Dice Die { get; set; }
        public int NumberOfDice { get; set; }
        [JsonConverter(typeof(DamageTypeConverter))]
        public DamageType DamageType { get; set; }
    
        public DamageResult GetResult(RollType rollType)
        {
            return new DamageResult(DiceUtils.RollMultiple(NumberOfDice, Die, rollType).Sum(), DamageType);
        }

        public override string ToString()
        {
            return $"{NumberOfDice}{Die} {DamageType.Name.ToLower()} damage";
        }
    }
}
