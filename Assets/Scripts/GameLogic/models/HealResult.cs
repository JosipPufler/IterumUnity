using Assets.Scripts.GameLogic.models.enums;

namespace Assets.Scripts.GameLogic.models
{
    public class HealResult
    {
        public HealResult(int amount, HealType healType)
        {
            Amount = amount;
            HealType = healType;
        }

        public int Amount { get; set; }
        public HealType HealType { get; set; }
    }
}
