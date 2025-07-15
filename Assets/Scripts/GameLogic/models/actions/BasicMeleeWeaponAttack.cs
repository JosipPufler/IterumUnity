using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.utils;
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
            ICreature originCreature = actionInfo.OriginCreature;

            if (targetCreature != null && ApCost <= originCreature.CurrentAp && MpCost <= originCreature.CurrentMp)
            {
                Stat baseStat = Stat.Strength;
                if (weapon.WeaponTraits.Contains(WeaponTrait.Finnes) && originCreature.GetAttributeModifier(Attribute.Agility) > originCreature.GetAttributeModifier(Attribute.Strength)) { 
                    baseStat = Stat.Agility;
                }

                CombatUtils.Attack(originCreature, targetCreature, AttackType.MeleeWeapon(baseStat, originCreature.ProficiencyManager.IsProficient(weapon)), new ActionPackage() { DamageOnSuccess = weapon.DamageInfos }, actionResultBuilder);
                return actionResultBuilder.Build();
            }
            return actionResultBuilder.Fail().Build();
        }
    }
}
