using Assets.Scripts.GameLogic.models.enums;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models.actions
{
    public class BasicWeaponAttack : IAction
    {
        public BasicWeaponAttack(IWeapon weapon, string name) : this(weapon, name, $"Attack an enemy with {name} dealing {weapon.GetDamageInfoString()}")
        {
        }

        public BasicWeaponAttack(IWeapon weapon, string name, string description)
        {
            this.weapon = weapon;
            Name = name;
            Description = description;
            ApCost = 1;
            Action = Func;
        }

        private readonly IWeapon weapon;

        public string Name { get; private set; }

        public string Description { get; private set; }

        public int ApCost { get; private set; }

        public IDictionary<TargetType, int> TargetTypes { get; } = new Dictionary<TargetType, int>() { {TargetType.Creature, 1} };

        public Func<ActionInfo, ActionResult> Action { get; private set; }

        public int MpCost { get; } = 0;

        ActionResult Func(ActionInfo actionInfo) {
            ITargetable targetable = actionInfo.Targets.FirstOrDefault(x => x.TargetType == TargetType.Creature);
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);

            if (targetable != null && targetable is ICreature targetCreature && ApCost <= actionInfo.OriginCreature.CurrentAp && MpCost <= actionInfo.OriginCreature.CurrentMp)
            {
                actionInfo.OriginCreature.CurrentAp -= ApCost;
                actionInfo.OriginCreature.CurrentMp -= MpCost;

                int attackModifier = actionInfo.OriginCreature.GetAttackModifier(Stat.Strength, AttackType.Weapon);
                if (weapon.WeaponTraits.Contains(WeaponTrait.Finnes) && actionInfo.OriginCreature.GetAttackModifier(Stat.Agility, AttackType.Weapon) > attackModifier)
                {
                    attackModifier = actionInfo.OriginCreature.GetAttackModifier(Stat.Agility, AttackType.Weapon);
                }
                int result = DiceUtils.Roll(Dice.d20, targetCreature.GetTargetRollType()) + attackModifier;
                if (result >= targetCreature.EvasionRating)
                {
                    IEnumerable<DamageResult> damage = weapon.DamageInfos.Select(info => info.GetResult(RollType.Normal));
                    actionResultBuilder.AmountDamaged(new List<KeyValuePair<IDamageable, IEnumerable<DamageResult>>>() { new(targetCreature, damage) });
                    targetCreature.TakeDamage(damage);
                }
                return actionResultBuilder.Build();
            }
            return actionResultBuilder.Fail().Build();
        }
    }
}
