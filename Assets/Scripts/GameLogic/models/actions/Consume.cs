using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;

namespace Iterum.models.actions
{
    public class Consume : IAction
    {
        public Consume(IConsumable consumable, string description, int apCost, int mpCost, IDictionary<TargetType, int> targetTypes, Func<ActionInfo, ActionResult> func) {
            Description = description;
            this.consumable = consumable;
            Name = $"Consume {consumable.Name}";
            ApCost = apCost;
            MpCost = mpCost;
            TargetTypes = targetTypes;
            Action = func;
        }

        IConsumable consumable;

        public string Name { get; }

        public string Description { get; }

        public int ApCost { get; }

        public IDictionary<TargetType, int> TargetTypes { get; }

        public Func<ActionInfo, ActionResult> Action { get; }

        public int MpCost { get; }
    }
}
