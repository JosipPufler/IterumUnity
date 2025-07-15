// Assets/Scripts/Utils/CustomBaseAction.cs
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Assets.Scripts.Utils.converters;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.GameLogic.models.actions
{
    public class CustomBaseAction : BaseAction
    {
        public string Id { get; set; }
        public CustomBaseAction(){}

        [JsonIgnore]
        public override Dictionary<TargetData, int> TargetTypes { get => CustomTargetTypes.ToDictionary(kvp => (TargetData)kvp.Key, kvp => kvp.Key.NumberOfTargets); }
        [JsonConverter(typeof(DictionaryKeyCustomTargetDataConverter))]
        public Dictionary<CustomTargetData, ActionPackage> CustomTargetTypes { get; set; } = new();

        public override ActionResult ExecuteAction(ActionInfo actionInfo)
        {
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);
            ICreature originCreature = actionInfo.OriginCreature;

            if (ValidateTargets(actionInfo) && CanTakeAction(actionInfo.OriginCreature))
            {
                actionInfo.OriginCreature.CurrentAp -= ApCost;
                actionInfo.OriginCreature.CurrentMp -= MpCost;

                foreach (CustomTargetData targetData in CustomTargetTypes.Keys)
                {
                    ActionPackage actionPackage = CustomTargetTypes[targetData];
                    // If targeting creature
                    if (targetData.TargetType == TargetType.Creature) {
                        List<ICreature> creatures = actionInfo.Targets[targetData].Select(x => (ICreature)x.Targetable).ToList();

                        foreach (ICreature target in creatures) {
                            // Command: Apply damage and modifiers
                            if (targetData.ActionType == ActionType.Command)
                            {
                                if (actionPackage.ModifiersOnSuccess != null)
                                {
                                    target.ModifierManager.ApplyModifiers(actionPackage.ModifiersOnSuccess);
                                    actionResultBuilder.AttributesModified(target, actionPackage.ModifiersOnSuccess);
                                }
                                if (actionPackage.DamageOnSuccess != null)
                                {
                                    IEnumerable<DamageResult> results = actionPackage.DamageOnSuccess.Select(x => x.GetResult(target.GetTargetRollType()));
                                    target.TakeDamage(results);
                                    actionResultBuilder.AmountDamaged(target, results);
                                }
                            }
                            else if (targetData.ActionType == ActionType.Attack)
                            {
                                CombatUtils.Attack(originCreature, target, targetData.AttackType, actionPackage, actionResultBuilder);
                            }
                            else if (targetData.ActionType == ActionType.SavingThrow)
                            {
                                CombatUtils.ForceSavingThrow(originCreature, target, targetData.SavingThrow, actionPackage, actionResultBuilder);
                            }
                        }
                    }

                    else if (targetData.TargetType == TargetType.Tile)
                    {
                        List<Vector3Int> creatures = actionInfo.Targets[targetData].Select(x => (Vector3Int)x.Targetable).ToList();

                        foreach (Vector3Int target in creatures)
                        {
                            // Command: Apply damage and modifiers
                            if (targetData.ActionType == ActionType.Command)
                            {
                                CombatUtils.ApplyDamageAndModifiersInAoe(originCreature, target, targetData.Radius, actionPackage.DamageOnSuccess, actionPackage.ModifiersOnSuccess, actionResultBuilder);
                            }
                            else if (targetData.ActionType == ActionType.Attack)
                            {
                                CombatUtils.AttackInAoe(originCreature, target, targetData.Radius, targetData.AttackType, actionPackage, actionResultBuilder);
                            }
                            else if (targetData.ActionType == ActionType.SavingThrow)
                            {
                                CombatUtils.ForceSavingThrowInAoe(originCreature, target, targetData.Radius, targetData.SavingThrow, actionPackage, actionResultBuilder);
                            }
                        }
                    }
                }
            }
            return actionResultBuilder.Build();
        }
    }
}
