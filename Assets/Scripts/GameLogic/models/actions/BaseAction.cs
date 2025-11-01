using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.Utils.converters;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using Mirror;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.GameLogic.models.actions
{
    public class BaseAction : IAction
    {
        public BaseAction() { }

        public BaseAction(BaseAction baseAction) { 
            ID = Guid.NewGuid().ToString();
            Name = baseAction.Name;
            Description = baseAction.Description;
            ApCost = baseAction.ApCost;
            MpCost = baseAction.MpCost;
            TargetTypes.AddRange(baseAction.TargetTypes);
            Action = baseAction.Action;
        }

        [JsonProperty]
        public virtual string ID { get; set; } = Guid.NewGuid().ToString();
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int ApCost { get; set; }
        public virtual int MpCost { get; set; }
        [JsonConverter(typeof(TargetDataDictionaryConverter))]
        public virtual Dictionary<TargetData, int> TargetTypes { get; set; } = new();
        [JsonIgnore]
        public virtual Func<ActionInfo, ActionResult> Action { get; set; }

        public virtual void Initialize() { }
        public virtual BaseAction Clone() { 
            return new BaseAction() {
                ID = Guid.NewGuid().ToString(),
                Name = Name,
                Description = Description,
                ApCost = ApCost,
                MpCost = MpCost,
                TargetTypes = TargetTypes,
                Action = Action
            };
        }

        public virtual int GetNumberOFTargets(IDictionary<TargetData, int> targetTypes)
        {
            int sum = 0;
            foreach (int numberOfSlots in targetTypes.Values)
            {
                sum += numberOfSlots;
            }
            return sum;
        }

        public virtual bool CanTakeAction(BaseCreature actionable) => actionable.CurrentAp >= ApCost && actionable.CurrentMp >= MpCost;

        public virtual bool ValidateTargets(ActionInfo actionInfo)
        {
            Dictionary<TargetData, int> targetSizes = actionInfo.Targets.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count);
            foreach (TargetData targetData in TargetTypes.Keys)
            {
                if (TargetTypes[targetData] != targetSizes[targetData])
                {
                    return false;
                }

                if ((targetData.TargetType == TargetType.Creature && actionInfo.Targets[targetData].Any(x => x.GetType() != typeof(TargetDataSubmissionCreature)))
                    || actionInfo.Targets[targetData].Count == 0)
                {
                    return false;
                }

                if ((targetData.TargetType == TargetType.Tile && actionInfo.Targets[targetData].Any(x => x.GetType() != typeof(TargetDataSubmissionHex)))
                    || actionInfo.Targets[targetData].Count == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public virtual ActionResult ExecuteAction(ActionInfo actionInfo)
        {
            if (ValidateTargets(actionInfo) && CanTakeAction(actionInfo.OriginCreature))
            {
                actionInfo.OriginCreature.CurrentAp -= ApCost;
                actionInfo.OriginCreature.CurrentMp -= MpCost;
                Debug.Log($"[ExecuteAction] {GetType().Name} | Action={(Action == null ? "null" : "set")}");
                return Action.Invoke(actionInfo);
            }
            return ActionResultBuilder.Start(actionInfo.OriginCreature).Fail().Build();
        }

        public virtual string GetCostString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"{ApCost}AP");
            if (MpCost > 0)
            {
                stringBuilder.Append($" & {MpCost}MP");
            }
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return $"<b>{Name}</b>: {Description}";
        }

        public string GetTooltipText()
        {
            StringBuilder sb = new();
            sb.AppendLine($"<size=175%><b>{Name}</b></size>");
            sb.AppendLine($"<size=150%>{GetCostString()}</size>");
            sb.AppendLine(Description);
            return sb.ToString();
        }
    }
}
