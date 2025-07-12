using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.target;
using Iterum.models.enums;
using Mirror.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.InputSystem.Controls;

namespace Iterum.models.interfaces
{
    public interface IAction
    {
        string Name { get; }
        string Description { get; }
        int ApCost { get; }
        int MpCost { get; }
        Dictionary<TargetData, int> TargetTypes { get; }
        Func<ActionInfo, ActionResult> Action { get; }

        int GetNumberOFTargets(IDictionary<TargetData, int> targetTypes);

        bool CanTakeAction(ICreature actionable);

        bool ValidateTargets(ActionInfo actionInfo);

        ActionResult ExecuteAction(ActionInfo actionInfo);

        string GetCostString();

        string GetLongDescription();
    }
}
