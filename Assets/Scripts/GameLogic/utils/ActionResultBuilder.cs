using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace Iterum.utils
{
    public class ActionResultBuilder
    {
        private ActionResultBuilder(IGameEntity gameEntity) {
            actionResult = new ActionResult(gameEntity);
        }

        private readonly ActionResult actionResult; 

        public static ActionResultBuilder Start(IGameEntity gameEntity) {
            return new ActionResultBuilder(gameEntity);
        }

        public ActionResultBuilder AmountHeald(IEnumerable<KeyValuePair<IDamageable, int>> healed)
        {
            actionResult.AmountHealed = actionResult.AmountHealed.ToArray().Concat(healed).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(x => x.Value));
            return this;
        }

        public ActionResultBuilder AmountHeald(IDamageable damageable, int amount)
        {
            if (actionResult.AmountHealed.TryGetValue(damageable, out int existingAmount))
            {
                actionResult.AmountHealed[damageable] = existingAmount + amount;
            }
            else
            {
                actionResult.AmountHealed.Add(damageable, amount);
            }
            return this;
        }

        public ActionResultBuilder AmountDamaged(IEnumerable<KeyValuePair<IDamageable, IEnumerable<DamageResult>>> damage)
        {
            actionResult.AmountDamaged = actionResult.AmountDamaged.ToArray().Concat(damage).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.SelectMany(x => x.Value));
            return this;
        }

        public ActionResultBuilder AmountDamaged(IDamageable target, IEnumerable<DamageResult> damage){
            return AmountDamaged(new[] { new KeyValuePair<IDamageable, IEnumerable<DamageResult>>(target, damage) });
        }

        public ActionResultBuilder AmountDamaged(IDamageable damageable, DamageResult damage)
        {
            if (actionResult.AmountDamaged.TryGetValue(damageable, out IEnumerable<DamageResult> value))
            {
                value.Append(damage);
            }
            else
            {
                actionResult.AmountDamaged.Add(damageable, new DamageResult[] { damage });
            }
            return this;
        }

        public ActionResultBuilder StatusEffectsApplied(IEnumerable<KeyValuePair<ICreature, HashSet<StatusEffect>>> effects)
        {
            actionResult.StatusEffectsApplied = actionResult.StatusEffectsApplied.ToArray().Concat(effects).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.SelectMany(x => x.Value).ToHashSet());
            return this;
        }

        public ActionResultBuilder StatusEffectsApplied(ICreature creature, StatusEffect statusEffect)
        {
            if (actionResult.StatusEffectsApplied.TryGetValue(creature, out HashSet<StatusEffect> value))
            {
                value.Append(statusEffect);
            }
            else
            {
                actionResult.StatusEffectsApplied.Add(creature, new HashSet<StatusEffect> { statusEffect });
            }
            return this;
        }

        public ActionResultBuilder AttributesModified(IEnumerable<KeyValuePair<ICreature, Dictionary<Attribute, int>>> attributes)
        {
            actionResult.AttributesModified = actionResult.AttributesModified.ToArray().Concat(attributes).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.SelectMany(x => x.Value).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(x => x.Value)));
            return this;
        }

        public ActionResultBuilder AttributesModified(ICreature creature, Dictionary<Attribute, int> attributes)
        {
            if (actionResult.AttributesModified.TryGetValue(creature, out Dictionary<Attribute, int> value))
            {
                value = value.ToArray().Concat(attributes).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(x => x.Value));
            }
            else
            {
                actionResult.AttributesModified.Add(creature, attributes);
            }
            return this;
        }

        public ActionResultBuilder Fail() { 
            actionResult.Success = false;
            return this;
        }

        public ActionResultBuilder AddMessage(string message)
        {
            actionResult.ActionMessages.Add(message);
            return this;
        }

        public ActionResult Build()
        {
            foreach (var damageResult in actionResult.AmountDamaged)
            {
                actionResult.ActionMessages.Add($"{actionResult.Source.Name} dealt {damageResult.Value.Sum(x => x.Amount)} damage to {damageResult.Key.Name}");
            }
            foreach (var modifiers in actionResult.AttributesModified)
            {
                foreach (var modifierResult in modifiers.Value)
                {
                    if (modifierResult.Value > 0)
                    {
                        actionResult.ActionMessages.Add($"{actionResult.Source.Name} increased {modifiers.Key.Name}s {modifierResult.Key} by {modifierResult.Value}");
                    } else if (modifierResult.Value < 0)
                    {
                        actionResult.ActionMessages.Add($"{actionResult.Source.Name} lowered {modifiers.Key.Name}s {modifierResult.Key} by {modifierResult.Value}");
                    }
                }
            }
            return actionResult;
        }
    }
}
