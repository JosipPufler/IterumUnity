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

        public int Amount { get; set; }
        public DamageType DamageType { get; set; }
    }
}
