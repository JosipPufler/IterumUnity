using Iterum.models.enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iterum.models.interfaces
{
    public interface IAction
    {
        string Name { get; }
        string Description { get; }
        int ApCost { get; }
        int MpCost { get; }
        IDictionary<TargetType, int> TargetTypes { get; }
        Func<ActionInfo, ActionResult> Action { get; }

        int GetNumberOFTargets(IDictionary<TargetType, int> targetTypes) {
            int sum = 0;
            foreach (int numberOfSlots in targetTypes.Values)
            {
                sum += numberOfSlots;
            }
            return sum;
        }

        bool CanTakeAction(IActionable actionable) { return actionable.CurrentAp >= ApCost; }

        bool ValidateTargets(ActionInfo actionInfo) {
            if (GetNumberOFTargets(TargetTypes) != actionInfo.Targets.Count)
            {
                return false;
            }
            Dictionary<TargetType, int> targets = new Dictionary<TargetType, int>();
            foreach (ITargetable item in actionInfo.Targets)
            {
                if (!targets.ContainsKey(item.TargetType))
                {
                    targets.Add(item.TargetType, 0);
                }
                targets[item.TargetType]++;    
            }
            foreach (KeyValuePair<TargetType, int> item in targets)
            {
                if (!TargetTypes.TryGetValue(item.Key, out int number) || item.Value != number)
                {
                    return false;
                }
            }
            return true;
        }

        ActionResult ExecuteAction(ActionInfo actionInfo) {
            if (ValidateTargets(actionInfo) && CanTakeAction(actionInfo.OriginCreature))
            {
                actionInfo.OriginCreature.CurrentAp -= ApCost;
                return Action.Invoke(actionInfo);
            }
            return null;
        }

        string GetCostString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{ApCost}AP");
            if (MpCost > 0)
            {
                stringBuilder.Append($" & {MpCost}MP");
            }
            return stringBuilder.ToString();
        }

        string GetLongDescription() { 
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<size=200%><b>{Name}</b></size>");
            sb.AppendLine($"<size=150%>{GetCostString()}</size>");
            sb.AppendLine(Description);
            return sb.ToString();
        }
    }
}
