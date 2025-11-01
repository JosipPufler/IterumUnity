using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.items;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.GameLogic.models.actions
{
    public class DelayedFireBlast : BaseAction
    {
        public DelayedFireBlast()
        {
            Name = "Delayed fire blast";
            Description = "Fire a blast of fire, if it hits the target trigger an additional explosion.";
            Action = Func;
            ApCost = 4;
            MpCost = 4;
        }

        readonly List<DamageInfo> BoltDamage = new() { new DamageInfo(2, Dice.d8, DamageType.Fire) } ;
        readonly List<DamageInfo> BlastDamageFull = new() { new DamageInfo(1, Dice.d8, DamageType.Fire) };
        readonly List<DamageInfo> BlastDamageHalved = new() { new DamageInfo(1, Dice.d8, DamageType.Fire, true) };

        public override Dictionary<TargetData, int> TargetTypes => new() {
            { new TargetData(TargetType.Creature, 0, 5), 1}
        };

        ActionResult Func(ActionInfo actionInfo)
        {
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);
            BaseCreature originCreature = actionInfo.OriginCreature;
            BaseCreature targetCreature = ((TargetDataSubmissionCreature)actionInfo.Targets.FirstOrDefault(x => x.Key.TargetType == TargetType.Creature).Value.First()).GetToken().creature;

            if (targetCreature != null)
            {
                bool attackHit = CombatUtils.Attack(originCreature, targetCreature, AttackType.SpellAttack(Stat.Intelligence, true), new ActionPackage() { DamageOnSuccess = BoltDamage }, actionResultBuilder);

                if (attackHit)
                {
                    CombatUtils.ForceSavingThrowInAoe(originCreature, targetCreature.CurrentPosition, 1, new SavingThrow(8, Stat.Intelligence, Stat.Agility), new ActionPackage() { DamageOnSuccess = BlastDamageFull, DamageOnFail = BlastDamageHalved }, actionResultBuilder);
                }

                return actionResultBuilder.Build();
            }
            return actionResultBuilder.Fail().Build();
        }
    }
}
