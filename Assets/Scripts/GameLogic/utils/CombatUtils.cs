using Assets.Scripts.GameLogic.models.enums;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogic.utils
{
    public class ActionPackage {
        public IList<DamageInfo> DamageOnSuccess { get; set; } = new List<DamageInfo>();
        public IList<DamageInfo> DamageOnFail { get; set; } = new List<DamageInfo>();
        public Dictionary<Attribute, int> ModifiersOnSuccess { get; set; } = new();
        public Dictionary<Attribute, int> ModifiersOnFail { get; set; } = new();

        public ActionPackage(){}

        public ActionPackage(List<DamageInfo> damageOnSuccess, List<DamageInfo> damageOnFail, Dictionary<Attribute, int> modifiersOnSuccess, Dictionary<Attribute, int> modifiersOnFail)
        {
            DamageOnSuccess = damageOnSuccess;
            DamageOnFail = damageOnFail;
            ModifiersOnSuccess = modifiersOnSuccess;
            ModifiersOnFail = modifiersOnFail;
        }
    }

    public static class CombatUtils
    {
        public static void AttackInAoe(ICreature source, Vector3Int point, int radius, AttackType attackType, ActionPackage actionPackage, ActionResultBuilder actionResultBuilder)
        {
            IEnumerable<ICreature> targets = CampaignGridLayout.GetTokensInRing(point, 0, radius).Select(x => x.creature);
            foreach (ICreature target in targets)
            {
                Attack(source, target, attackType, actionPackage, actionResultBuilder);
            }
        }

        public static void ForceSavingThrowInAoe(ICreature source, Vector3Int point, int radius, SavingThrow savingThrow, ActionPackage actionPackage, ActionResultBuilder actionResultBuilder)
        {
            IEnumerable<ICreature> targets = CampaignGridLayout.GetTokensInRing(point, 0, radius).Select(x => x.creature);
            foreach (ICreature target in targets)
            {
                ForceSavingThrow(source, target, savingThrow, actionPackage, actionResultBuilder);
            }
        }

        public static void Attack(ICreature source, ICreature target, AttackType attackType, ActionPackage actionPackage, ActionResultBuilder actionResultBuilder) {
            int rollResult = DiceUtils.Roll(Dice.d20);
            int attackRollResult = rollResult + source.ModifierManager.GetAttackModifier(attackType);
            if (attackRollResult > target.EvasionRating)
            {
                actionResultBuilder.AddMessage($"{source.Name} hit {target.Name} with a {attackType.Name} ({attackRollResult} vs {target.EvasionRating})");
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnSuccess, actionPackage.ModifiersOnSuccess, actionResultBuilder, attackType);
            }
            else
            {
                actionResultBuilder.AddMessage($"{source.Name} missed {target.Name} with a {attackType.Name} ({attackRollResult} vs {target.EvasionRating})");
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnFail, actionPackage.ModifiersOnFail, actionResultBuilder, attackType);
            }
        }

        public static void ForceSavingThrow(ICreature source, ICreature target, SavingThrow savingThrow, ActionPackage actionPackage, ActionResultBuilder actionResultBuilder)
        {
            if (source.GetSaveDC(savingThrow) > target.SavingThrow(savingThrow.SaveStat, RollType.Normal))
            {
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnSuccess, actionPackage.ModifiersOnSuccess, actionResultBuilder);
            }
            else
            {
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnFail, actionPackage.ModifiersOnFail, actionResultBuilder);
            }
        }

        public static void ApplyDamageAndModifiersInAoe(ICreature source, Vector3Int point, int radius, IList<DamageInfo> damageInfos, Dictionary<Attribute, int> modifiers, ActionResultBuilder actionResultBuilder, AttackType attackType = null)
        {
            IEnumerable<ICreature> creatures = CampaignGridLayout.GetTokensInRing(point, 0, radius).Select(x => x.creature);
            foreach (var creature in creatures)
            {
                ApplyDamageAndModifiers(source, creature, damageInfos, modifiers, actionResultBuilder, attackType);
            }
        }

        private static void ApplyDamageAndModifiers(ICreature source, ICreature target, IList<DamageInfo> damageInfos, Dictionary<Attribute, int> modifiers, ActionResultBuilder actionResultBuilder, AttackType attackType = null)
        {
            if (damageInfos != null && damageInfos.Count > 0)
            {
                DealDamage(source, target, damageInfos, actionResultBuilder, attackType);
            }
            if (modifiers != null)
            {
                target.ModifierManager.ApplyModifiers(modifiers);
                actionResultBuilder.AttributesModified(target, modifiers);
            }
        }

        private static void DealDamage(ICreature source, ICreature target, IList<DamageInfo> damageInfo, ActionResultBuilder actionResultBuilder, AttackType attackType = null)
        {
            List<DamageResult> damageResults = damageInfo.Select(x =>
            {
                DamageResult damageResult = x.GetResult(RollType.Normal);
                damageResult.Amount += source.ModifierManager.GetDamageModifierForDamageType(x.DamageType);
                return damageResult;
            }).ToList();
            DamageResult maxDamageResult = damageResults.OrderByDescending(x => x.Amount).FirstOrDefault();
            maxDamageResult.Amount += source.GetAttributeModifier(Attribute.GlobalDamage);
            if (attackType != null)
                maxDamageResult.Amount += source.ModifierManager.GetAttackDamageModifier(attackType);
            int damage = target.TakeDamage(damageResults);
            actionResultBuilder.AmountDamaged(target, damageResults);
        }
    }
}
