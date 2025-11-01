using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.items;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models.actions
{
    public class BasicMeleeWeaponAttack : WeaponAction
    {
        public BasicMeleeWeaponAttack() {
            Initialize();
        }

        public BasicMeleeWeaponAttack(BaseWeapon weapon, string name) : this(weapon, name, $"Attack an enemy with {name} dealing {weapon.GetDamageInfoString()}"){}

        public BasicMeleeWeaponAttack(BaseWeapon weapon, string name, string description)
        {
            Weapon = weapon;
            Name = name;
            Description = description;
            ApCost = weapon.WeaponTraits.Contains(WeaponTrait.Heavy) ? 3 : 2;
            
            Initialize();
        }

        public override void Initialize() {
            Action = Func;
        }

        public override BaseAction Clone()
        {
            return new BasicMeleeWeaponAttack(Weapon, Name, Description);
        }

        [JsonIgnore]
        public override Dictionary<TargetData, int> TargetTypes => new() {
            { new TargetData(TargetType.Creature, 0, Weapon.ReachModifier + (Weapon.Creature != null ? Weapon.Creature.GetAttributeModifier(Attribute.Reach) : 0)), 1}
        };

        ActionResult Func(ActionInfo actionInfo) {
            BaseCreature targetCreature = ((TargetDataSubmissionCreature)actionInfo.Targets.FirstOrDefault(x => x.Key.TargetType == TargetType.Creature).Value.First()).GetToken().creature;
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);
            BaseCreature originCreature = actionInfo.OriginCreature;

            if (targetCreature != null)
            {
                Stat baseStat = Stat.Strength;
                if (Weapon.WeaponTraits.Contains(WeaponTrait.Finesse) && originCreature.GetAttributeModifier(Attribute.Agility) > originCreature.GetAttributeModifier(Attribute.Strength)) { 
                    baseStat = Stat.Agility;
                }

                CombatUtils.Attack(originCreature, targetCreature, AttackType.MeleeWeapon(baseStat, originCreature.ProficiencyManager.IsProficient(Weapon)), new ActionPackage() { DamageOnSuccess = Weapon.DamageInfos }, actionResultBuilder);
                return actionResultBuilder.Build();
            }
            return actionResultBuilder.Fail().Build();
        }
    }
}
