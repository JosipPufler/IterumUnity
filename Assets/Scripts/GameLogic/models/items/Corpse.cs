using Iterum.models.interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Iterum.models.items
{
    public class Corpse : IDamageable, IGameObject, IContainer
    {
        private readonly ICreature creature;
        public Corpse(ICreature creature)
        {
            this.creature = creature;
            CurrentPosition = creature.CurrentPosition;
            Name = $"Corpse of {creature.Name}";
            OriginalMaxHp = creature.OriginalMaxHp / 4;
            MaxHp = creature.MaxHp;
            CurrentHp = MaxHp;
        }

        public Vector3Int CurrentPosition { get; set; }
        public string Name { get; }
        public int CurrentHp { get; set; }
        public int MaxHp { get; set; }
        public int OriginalMaxHp { get; }
        public IDictionary<IItem, int> Inventory { get; }

        public void Destroy()
        {
            Inventory[new CorpseRation(creature)] = OriginalMaxHp / 25;
            /*for (int i = 0; i < OriginalMaxHp/25; i++)
            {
                Inventory.Add(new CorpseRation(creature));
            }*/
        }

        public void TakeDamage(IEnumerable<DamageResult> damage)
        {
            IDictionary<enums.DamageType, double> resistances = creature.Race.GetEffectiveDamageResistances();
            IDictionary<enums.DamageCategory, double> categoryResistances = creature.Race.GetEffectiveDamageCategoryResistances();
            foreach (DamageResult damageResult in damage)
            {

                CurrentHp = (int)Math.Ceiling(CurrentHp -
                    damageResult.Amount
                        * (categoryResistances.TryGetValue(damageResult.DamageType.DamageCategory, out double categoryValue) ? categoryValue : 1)
                        * (resistances.TryGetValue(damageResult.DamageType, out double typeValue) ? typeValue : 1));
                if (CurrentHp <= 0)
                {
                    Destroy();
                }
            }
        }

        #nullable enable
        public ICreature? Ressurect(int newHp) {
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
