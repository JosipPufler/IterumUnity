using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.interfaces;
using System;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IAction : ITooltipTarget
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
        int ApCost { get; }
        int MpCost { get; }
        Dictionary<TargetData, int> TargetTypes { get; }
        Func<ActionInfo, ActionResult> Action { get; }

        int GetNumberOFTargets(IDictionary<TargetData, int> targetTypes);

        bool CanTakeAction(BaseCreature actionable);

        bool ValidateTargets(ActionInfo actionInfo);

        ActionResult ExecuteAction(ActionInfo actionInfo);

        string GetCostString();
    }
}
