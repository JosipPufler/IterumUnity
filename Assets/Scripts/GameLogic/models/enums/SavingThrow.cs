// Assets/Scripts/Utils/SavingThrow.cs
using Assets.Scripts.Utils.converters;
using Iterum.models.enums;
using Newtonsoft.Json;

namespace Assets.Scripts.GameLogic.models.enums
{
    public class SavingThrow
    {
        public SavingThrow(Stat originStat, Stat saveStat) : this(8, originStat, saveStat) {}

        public SavingThrow(int baseDC, Stat originStat, Stat saveStat)
        {
            BaseDC = baseDC;
            OriginStat = originStat;
            SaveStat = saveStat;
        }

        public int BaseDC { get; set; }
        [JsonConverter(typeof(StatConverter))]
        public Stat OriginStat { get; set; }
        [JsonConverter(typeof(StatConverter))]
        public Stat SaveStat { get; set; }
    }
}
