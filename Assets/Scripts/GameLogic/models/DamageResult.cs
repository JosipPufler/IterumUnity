using Iterum.models.enums;

namespace Iterum.models
{
    public class DamageResult
    {
        public DamageResult(int amount, DamageType damageType)
        {
            Amount = amount;
            DamageType = damageType;
        }

        public DamageResult(int amount, DamageType damageType, int originalAmount) : this(amount, damageType){
            OriginalAmount = originalAmount;
        }

        public int Amount { get; set; }
        public int? OriginalAmount { get; set; } = null;
        public DamageType DamageType { get; set; }

        public override string ToString()
        {
            if (OriginalAmount != null)
                return $"{Amount} {DamageType.Name} damage (reduced from {OriginalAmount})";
            return $"{Amount} {DamageType.Name} damage";
        }
    }
}
