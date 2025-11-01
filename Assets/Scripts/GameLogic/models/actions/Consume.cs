using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Iterum.models.actions
{
    public class Consume : BaseAction
    {
        public Consume(BaseConsumable consumable, string description, int apCost, int mpCost, Dictionary<TargetData, int> targetTypes, Func<ActionInfo, ActionResult> func) {
            Description = description;
            this.consumable = consumable;
            Name = $"Consume {consumable.Name}";
            ApCost = apCost;
            MpCost = mpCost;
            TargetTypes = targetTypes;
            Action = func;
        }

        public Consume(BaseConsumable consumable, BaseAction baseAction) : this(consumable, baseAction.Description, baseAction.ApCost, baseAction.MpCost, baseAction.TargetTypes, baseAction.Action) { 
            
        }

        [JsonIgnore]
        public BaseConsumable consumable;
    }
}
