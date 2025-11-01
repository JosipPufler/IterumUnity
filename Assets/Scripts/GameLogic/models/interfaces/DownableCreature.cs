using Assets.Scripts.GameLogic.models.races;
using Iterum.models.enums;
using Iterum.utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iterum.models.interfaces
{
    public class DownableCreature : BaseCreature
    {
        public DownableCreature(BaseRace race, string name, string imagePath = "Textures/default", string description = "") : base(race, name, imagePath, description){}

        public List<bool> DeathSaves { get; } = new List<bool>();
        public bool IsDown { get; set; }

        public new void TakeDamage(IEnumerable<DamageResult> damage)
        {
            ((BaseCreature)this).TakeDamage(damage);
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

        public override string GetStatString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(base.GetStatString());
            if (DeathSaves.Count > 0)
            {
                int successes = DeathSaves.Count(x => x);
                int fails = DeathSaves.Count - successes;
                stringBuilder.AppendLine($"Death saves:");
                stringBuilder.AppendLine($"successes: {successes}/3");
                stringBuilder.AppendLine($"fails: {fails}/3");
            }
            return stringBuilder.ToString();
        }
    }
}
