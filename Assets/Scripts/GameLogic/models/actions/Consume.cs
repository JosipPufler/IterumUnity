using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;

namespace Iterum.models.actions
{
    public class Consume : BaseAction
    {
        public Consume(IConsumable consumable, string description, int apCost, int mpCost, Dictionary<TargetData, int> targetTypes, Func<ActionInfo, ActionResult> func) {
            Description = description;
            this.consumable = consumable;
            Name = $"Consume {consumable.Name}";
            ApCost = apCost;
            MpCost = mpCost;
            TargetTypes = targetTypes;
            Action = func;
        }

        IConsumable consumable;
    }
}
