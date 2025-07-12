using Assets.Scripts.GameLogic.models.enums;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.GameLogic.utils
{
    public class ActionPackage { 
        public List<DamageInfo> DamageOnSuccess { get; set; }
        public List<DamageInfo> DamageOnFail { get; set; }
        public Dictionary<Attribute, int> ModifiersOnSuccess { get; set; }
        public Dictionary<Attribute, int> ModifiersOnFail { get; set; }

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
        public static void Attack(ICreature source, ICreature target, AttackType attackType, ActionPackage actionPackage) {
            int attackRollResult = DiceUtils.Roll(Dice.d20) + source.ModifierManager.GetAttackModifier(attackType);
            if (attackRollResult > target.EvasionRating)
            {
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnSuccess, actionPackage.ModifiersOnSuccess, attackType);
            }
            else
            {
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnFail, actionPackage.ModifiersOnFail, attackType);
            }
        }

        public static void ForceSavingThrow(ICreature source, ICreature target, Stat savingThrowStat, ActionPackage actionPackage)
        {
            if (source.GetSaveDC(savingThrowStat) > target.SavingThrow(savingThrowStat, RollType.Normal))
            {
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnSuccess, actionPackage.ModifiersOnSuccess);
            }
            else
            {
                ApplyDamageAndModifiers(source, target, actionPackage.DamageOnFail, actionPackage.ModifiersOnFail);
            }
        }

        private static void ApplyDamageAndModifiers(ICreature source, ICreature target, List<DamageInfo> damageInfos, Dictionary<Attribute, int> modifiers, AttackType attackType = null)
        {
            if (damageInfos != null && damageInfos.Count > 0)
            {
                DealDamage(source, target, damageInfos, attackType);
            }
            if (modifiers != null)
            {
                target.ModifierManager.ApplyModifiers(modifiers);
            }
        }

        private static void DealDamage(ICreature source, ICreature target, List<DamageInfo> damageInfo, AttackType attackType = null)
        {
            IEnumerable<DamageResult> damageResults = damageInfo.Select(x =>
            {
                DamageResult damageResult = x.GetResult(RollType.Normal);
                damageResult.Amount += source.ModifierManager.GetDamageModifierForDamageType(x.DamageType);
                return damageResult;
            });
            DamageResult maxDamageResult = damageResults.OrderByDescending(x => x.Amount).FirstOrDefault();
            maxDamageResult.Amount += source.GetAttributeModifier(Attribute.GlobalDamage);
            if (attackType != null)
                maxDamageResult.Amount += source.ModifierManager.GetAttackDamageModifier(attackType);
            target.TakeDamage(damageResults);
        }
    }
}
