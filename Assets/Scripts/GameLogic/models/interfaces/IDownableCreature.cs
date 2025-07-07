using Iterum.models.enums;
using Iterum.utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Iterum.models.interfaces
{
    public abstract class IDownableCreature : ICreature
    {
        protected IDownableCreature(IRace race, string name, Vector3Int position, string imagePath = null) : base(race, name, position, imagePath)
        {
        }

        public List<bool> DeathSaves { get; }
        public bool IsDown { get; set; }

        public new void TakeDamage(IEnumerable<DamageResult> damage)
        {
            ((ICreature)this).TakeDamage(damage);
            if (CurrentHp <= 0)
            {
                CurrentHp = 0;
                GoDown();
            }
        }

        public void GoDown()
        {
            IsDown = true;
        }

        public void Stablize()
        {
            DeathSaves.Clear();
        }

        public int RollDeathSave(RollType rollType)
        {
            int result = DiceUtils.Roll(Dice.d20, rollType);
            if (result >= 10)
            {
                DeathSaves.Add(true);
                if (DeathSaves.Count(x => x) >= 3)
                {
                    Stablize();
                }
            }
            else
            {
                DeathSaves.Add(false);
                if (DeathSaves.Count(x => !x) >= 3)
                {
                    Die();
                }
            }
            return result;
        }
    }
}
