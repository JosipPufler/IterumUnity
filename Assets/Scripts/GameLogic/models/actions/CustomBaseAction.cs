// Assets/Scripts/Utils/CustomBaseAction.cs
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.target;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;

namespace Assets.Scripts.GameLogic.models.actions
{
    public class CustomBaseAction : BaseAction
    {
        public override Dictionary<TargetData, int> TargetTypes { get => CustomTargetTypes.ToDictionary(kvp => (TargetData)kvp.Key, kvp => kvp.Value); }
        public Dictionary<CustomTargetData, int> CustomTargetTypes { get; set; }
        public Dictionary<CustomTargetData, Dictionary<Attribute, int>> ModifiersOnSuccess { get; set; }
        public Dictionary<CustomTargetData, Dictionary<Attribute, int>> ModifiersOnFail { get; set; }
        public Dictionary<CustomTargetData, List<DamageInfo>> DamageOnSuccess { get; set; }
        public Dictionary<CustomTargetData, List<DamageInfo>> DamageOnFail { get; set; }

        public override ActionResult ExecuteAction(ActionInfo actionInfo)
        {
            if (ValidateTargets(actionInfo) && CanTakeAction(actionInfo.OriginCreature))
            {
                actionInfo.OriginCreature.CurrentAp -= ApCost;
                actionInfo.OriginCreature.CurrentMp -= MpCost;

                foreach (CustomTargetData targetData in CustomTargetTypes.Keys) 
                {
                    // If targeting creature
                    if (targetData.TargetType == TargetType.Creature) {
                        List<ICreature> creatures = actionInfo.Targets[targetData].Select(x => (ICreature)x.Targetable).ToList();

                        foreach (ICreature creature in creatures) {
                            // Command: Apply damage and modifiers
                            if (targetData.ActionType == ActionType.Command)
                            {
                                if (ModifiersOnSuccess.TryGetValue(targetData, out Dictionary<Attribute, int> modifiers))
                                {
                                    creature.ModifierManager.ApplyModifiers(modifiers);
                                }
                                if (DamageOnSuccess.TryGetValue(targetData, out List<DamageInfo> damage))
                                {
                                    damage.Select(x => x.GetResult(creature.GetTargetRollType()));
                                    creature.TakeDamage(damage);
                                }
                            }
                            else if (targetData.ActionType == ActionType.Attack)
                            {

                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
