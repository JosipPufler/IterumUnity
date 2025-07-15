using Assets.Scripts.GameLogic.models.target;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GameLogic.models.actions
{
    public abstract class BaseAction : IAction
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int ApCost { get; set; }
        public virtual int MpCost { get; set; }
        public virtual Dictionary<TargetData, int> TargetTypes { get; set; }
        public virtual Func<ActionInfo, ActionResult> Action { get; set; }

        public virtual int GetNumberOFTargets(IDictionary<TargetData, int> targetTypes)
        {
            int sum = 0;
            foreach (int numberOfSlots in targetTypes.Values)
            {
                sum += numberOfSlots;
            }
            return sum;
        }

        public virtual bool CanTakeAction(ICreature actionable) => actionable.CurrentAp >= ApCost && actionable.CurrentMp >= MpCost;

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
                return Action.Invoke(actionInfo);
            }
            return null;
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

        public virtual string GetLongDescription()
        {
            StringBuilder sb = new();
            sb.AppendLine($"<size=200%><b>{Name}</b></size>");
            sb.AppendLine($"<size=150%>{GetCostString()}</size>");
            sb.AppendLine(Description);
            return sb.ToString();
        }
    }
}
