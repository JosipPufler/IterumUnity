using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
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
    public class BasicMeleeWeaponAttack : BaseAction
    {
        public BasicMeleeWeaponAttack(IWeapon weapon, string name) : this(weapon, name, $"Attack an enemy with {name} dealing {weapon.GetDamageInfoString()}")
        {
        }

        public BasicMeleeWeaponAttack(IWeapon weapon, string name, string description)
        {
            this.weapon = weapon;
            Name = name;
            Description = description;
            ApCost = 1;
            Action = Func;
        }

        private readonly IWeapon weapon;

        public override Dictionary<TargetData, int> TargetTypes => new() { 
            { new TargetData(TargetType.Creature, 0, 1 + weapon.ReachModifier + weapon.Creature.GetAttributeModifier(Attribute.Reach, false)), 1} 
        };

        ActionResult Func(ActionInfo actionInfo) {
            ICreature targetCreature = (ICreature)actionInfo.Targets.FirstOrDefault(x => x.Key.TargetType == TargetType.Creature).Value.First().Targetable;
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);

            if (targetCreature != null && ApCost <= actionInfo.OriginCreature.CurrentAp && MpCost <= actionInfo.OriginCreature.CurrentMp)
            {
                actionInfo.OriginCreature.CurrentAp -= ApCost;
                actionInfo.OriginCreature.CurrentMp -= MpCost;

                int attackModifier = actionInfo.OriginCreature.GetAttackModifier(Stat.Strength, AttackType.MeleeWeapon);
                if (weapon.WeaponTraits.Contains(WeaponTrait.Finnes) && actionInfo.OriginCreature.GetAttackModifier(Stat.Agility, AttackType.MeleeWeapon) > attackModifier)
                {
                    attackModifier = actionInfo.OriginCreature.GetAttackModifier(Stat.Agility, AttackType.MeleeWeapon);
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
