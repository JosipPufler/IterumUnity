using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IDamageable : IGameEntity
    {
        int CurrentHp { get; set; }
        int MaxHp { get; set; }
        int OriginalMaxHp { get; }
        int TakeDamage(IEnumerable<DamageResult> damage);
        void Heal(int ammount) {
            if (ammount > MaxHp)
            {
                CurrentHp = MaxHp;
            }
            else
            {
                CurrentHp = ammount;
            }
        }
        
    }
}
