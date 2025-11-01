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
        public static void AttackInAoe(BaseCreature source, GridCoordinate point, int radius, AttackType attackType, ActionPackage actionPackage, ActionResultBuilder actionResultBuilder)
        {
            IEnumerable<BaseCreature> targets = CampaignGridLayout.Instance.GetTokensInRing(point, 0, radius).Select(x => x.creature);
            foreach (BaseCreature target in targets)
            {
                Attack(source, target, attackType, actionPackage, actionResultBuilder);
            }
        }

        public static void ForceSavingThrowInAoe(BaseCreature source, GridCoordinate point, int radius, SavingThrow savingThrow, ActionPackage actionPackage, ActionResultBuilder actionResultBuilder)
        {
            Debug.Log($"[AOE SavingThrow] {source.Name} is forcing a {savingThrow.SaveStat} saving throw on all creatures within radius {radius} of {point}.");

            IEnumerable<BaseCreature> targets = CampaignGridLayout.Instance.GetTokensInRing(point, 0, radius).Select(x => x.creature);
            int count = 0;
            foreach (BaseCreature target in targets)
            {
                if (target == null)
                {
                    Debug.LogWarning("[AOE SavingThrow] Found null creature in target list. Skipping.");
                    continue;
                }

                Debug.Log($"[AOE SavingThrow] Targeting {target.Name} at position {target.CurrentPosition}.");
                ForceSavingThrow(source, target, savingThrow, actionPackage, actionResultBuilder);
                count++;
            }
        }

        public static bool Attack(BaseCreature source, BaseCreature target, AttackType attackType, ActionPackage actionPackage, ActionResultBuilder actionResultBuilder) {
            int rollResult = DiceUtils.Roll(Dice.d20);
            int attackRollResult = rollResult + source.GetAttackModifier(attackType);
            if (attackRollResult >= target.EvasionRating)
            {
                actionResultBuilder.AddMessage($"{source.Name} hit {target.Name} with a {attackType.Name} ({attackRollResult} vs {target.EvasionRating})");
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnSuccess, actionPackage.ModifiersOnSuccess, actionResultBuilder, attackType);
                return true;
            }
            else
            {
                actionResultBuilder.AddMessage($"{source.Name} missed {target.Name} with a {attackType.Name} ({attackRollResult} vs {target.EvasionRating})");
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnFail, actionPackage.ModifiersOnFail, actionResultBuilder, attackType);
                return false;
            }
        }

        public static bool ForceSavingThrow(BaseCreature source, BaseCreature target, SavingThrow savingThrow, ActionPackage actionPackage, ActionResultBuilder actionResultBuilder)
        {
            float dc = source.GetSaveDC(savingThrow);
            float roll = target.SavingThrow(savingThrow.SaveStat, RollType.Normal);
            Debug.Log($"[SavingThrow] {target.Name} is making a {savingThrow.SaveStat} saving throw against {source.Name}'s DC {dc}. Rolled: {roll}");

            if (dc > roll)
            {
                actionResultBuilder.AddMessage($"{target.Name} failed a {savingThrow.SaveStat.Name} save from {source.Name} ({roll} vs {dc})");
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnSuccess, actionPackage.ModifiersOnSuccess, actionResultBuilder);
                return true;
            }
            else
            {
                actionResultBuilder.AddMessage($"{target.Name} succeeded a {savingThrow.SaveStat.Name} save from {source.Name} ({roll} vs {dc})");
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnFail, actionPackage.ModifiersOnFail, actionResultBuilder);
                return false;
            }
        }

        public static void ApplyDamageAndModifiersInAoe(BaseCreature source, GridCoordinate point, int radius, IList<DamageInfo> damageInfos, Dictionary<Attribute, int> modifiers, ActionResultBuilder actionResultBuilder, AttackType attackType = null)
        {
            IEnumerable<BaseCreature> creatures = CampaignGridLayout.Instance.GetTokensInRing(point, 0, radius).Select(x => x.creature);
            foreach (var creature in creatures)
            {
                ApplyDamageAndModifiers(source, creature, damageInfos, modifiers, actionResultBuilder, attackType);
            }
        }

        private static void ApplyDamageAndModifiers(BaseCreature source, BaseCreature target, IList<DamageInfo> damageInfos, Dictionary<Attribute, int> modifiers, ActionResultBuilder actionResultBuilder, AttackType attackType = null)
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

        private static void DealDamage(BaseCreature source, BaseCreature target, IList<DamageInfo> damageInfo, ActionResultBuilder actionResultBuilder, AttackType attackType = null)
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
            actionResultBuilder.AmountDamaged(target, target.TakeDamage(damageResults));
        }
    }
}
