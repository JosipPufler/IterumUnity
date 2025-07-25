using Assets.Scripts;
using Assets.Scripts.GameLogic.models.interfaces;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Iterum.models.items
{
    public class Corpse : IDamageable, IGameObject, IContainer
    {
        private readonly BaseCreature creature;
        public Corpse(BaseCreature creature)
        {
            this.creature = creature;
            CurrentPosition = creature.CurrentPosition;
            Name = $"Corpse of {creature.Name}";
            OriginalMaxHp = creature.OriginalMaxHp / 4;
            MaxHp = creature.MaxHp;
            CurrentHp = MaxHp;
        }

        public GridCoordinate CurrentPosition { get; set; }
        public string Name { get; }
        public int CurrentHp { get; set; }
        public int MaxHp { get; set; }
        public int OriginalMaxHp { get; }
        public List<BaseItem> Inventory { get; }

        public void Destroy()
        {
            for (int i = 0; i < OriginalMaxHp/25; i++)
            {
                Inventory.Add(new CorpseRation(creature));
            }
        }

        public int TakeDamage(IEnumerable<DamageResult> damage)
        {
            IDictionary<DamageType, double> resistances = creature.Race.GetEffectiveDamageResistances();
            IDictionary<DamageCategory, double> categoryResistances = creature.Race.GetEffectiveDamageCategoryResistances();
            int totalDamageTaken = 0;
            foreach (DamageResult damageResult in damage)
            {
                int damageTaken = (int)Math.Ceiling(damageResult.Amount
                        * (categoryResistances.TryGetValue(damageResult.DamageType.DamageCategory, out double categoryValue) ? categoryValue : 1)
                        * (resistances.TryGetValue(damageResult.DamageType, out double typeValue) ? typeValue : 1));
                totalDamageTaken += damageTaken;
                CurrentHp -= damageTaken;
                if (CurrentHp <= 0)
                {
                    Destroy();
                }
            }
            return totalDamageTaken;
        }

        #nullable enable
        public BaseCreature? Ressurect(int newHp) {
            if (newHp >= 1)
            {
                creature.CurrentHp = newHp;
                return creature;
            }
            return null;
        }
        #nullable disable
    }
}
