using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.items;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System.Collections.Generic;
using System.Linq;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models.actions
{
    public class BasicMeleeWeaponAttack : WeaponAction
    {
        public BasicMeleeWeaponAttack(BaseWeapon weapon, string name) : this(weapon, name, $"Attack an enemy with {name} dealing {weapon.GetDamageInfoString()}"){
            Initialize();
        }

        public BasicMeleeWeaponAttack(BaseWeapon weapon, string name, string description)
        {
            this.weapon = weapon;
            Name = name;
            Description = description;
            ApCost = 1;
            Initialize();
        }

        public override void Initialize() {
            Action = Func;
        }

        public override Dictionary<TargetData, int> TargetTypes => new() { 
            { new TargetData(TargetType.Creature, 0, 1 + weapon.ReachModifier + weapon.Creature.GetAttributeModifier(Attribute.Reach)), 1} 
        };

        ActionResult Func(ActionInfo actionInfo) {
            BaseCreature targetCreature = ((TargetDataSubmissionCreature)actionInfo.Targets.FirstOrDefault(x => x.Key.TargetType == TargetType.Creature).Value.First()).GetToken().creature;
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);
            BaseCreature originCreature = actionInfo.OriginCreature;

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
