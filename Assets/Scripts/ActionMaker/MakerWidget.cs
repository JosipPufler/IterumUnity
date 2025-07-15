using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Iterum.models;
using Iterum.models.enums;
using kcp2k;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ActionMaker
{
    public class MakerWidget : DraggableUI
    {
        [Header("Inputs")]
        public TMP_InputField ifMinDistance;
        public TMP_InputField ifMaxDistance;
        public TMP_Dropdown targetTypeDropdown;
        public TMP_InputField ifRadius;
        public TMP_InputField ifNumberOfTargets;
        public GameObject radiusRow;
        public GameObject numberOfTargetsRow;
        public GameObject content;

        public GameObject damageWidgetPrefab;
        public GameObject modifierWidgetPrefab;

        protected readonly List<TargetType> supportedTargetTypes = new() { TargetType.Creature, TargetType.Tile };

        bool initialized = false;

        public virtual CustomTargetData GetCustomTargetData()
        {
            if (supportedTargetTypes[targetTypeDropdown.value] == TargetType.Tile)
            {
                return new CustomTargetData(supportedTargetTypes[targetTypeDropdown.value], int.Parse(ifMinDistance.text), int.Parse(ifMaxDistance.text), int.Parse(ifRadius.text));
            }
            else
            {
                return new CustomTargetData(supportedTargetTypes[targetTypeDropdown.value],int.Parse(ifMinDistance.text), int.Parse(ifMaxDistance.text), int.Parse(ifNumberOfTargets.text));
            }
        }

        public virtual ActionPackage GetActionPackage() {
            ActionPackage actionPackage = new();
            foreach (Transform child in content.transform)
            {
                if (child.TryGetComponent<AttributeWidget>(out var attributeWidget))
                {
                    KeyValuePair<bool, Dictionary<Iterum.models.enums.Attribute, int>> keyValuePair = attributeWidget.GetAttributes();
                    var targetDict = keyValuePair.Key ? actionPackage.ModifiersOnSuccess : actionPackage.ModifiersOnFail;

                    foreach (var kvp in keyValuePair.Value)
                    {
                        if (targetDict.ContainsKey(kvp.Key))
                            targetDict[kvp.Key] += kvp.Value;
                        else
                            targetDict[kvp.Key] = kvp.Value;
                    }
                }

                if (child.TryGetComponent<DamageWidget>(out var damageWidget))
                {
                    KeyValuePair<bool, List<DamageInfo>> keyValuePair = damageWidget.GetDamage();
                    if (keyValuePair.Key)
                    {
                        actionPackage.DamageOnSuccess = actionPackage.DamageOnSuccess.Concat(keyValuePair.Value).ToList();
                    }
                    else
                    {
                        actionPackage.DamageOnFail = actionPackage.DamageOnFail.Concat(keyValuePair.Value).ToList();
                    }
                }
            }
            return actionPackage;
        }

        protected void Start()
        {
            if (!initialized)
            {
                Init();
            }
        }

        protected virtual void Init()
        {
            radiusRow.SetActive(false);

            targetTypeDropdown.ClearOptions();
            var typeNames = supportedTargetTypes.Select(t => t.ToString()).ToList();
            targetTypeDropdown.AddOptions(typeNames);
            targetTypeDropdown.onValueChanged.AddListener(index =>
            {
                if (supportedTargetTypes[index] == TargetType.Tile)
                {
                    radiusRow.SetActive(true);
                    numberOfTargetsRow.SetActive(false);
                }
                else
                {
                    radiusRow.SetActive(false);
                    numberOfTargetsRow.SetActive(true);
                }
            });
            initialized = true;
        }

        public virtual void LoadAction(KeyValuePair<CustomTargetData, ActionPackage> actionData)
        {
            Init();
            CustomTargetData targetData = actionData.Key;
            ActionPackage actionPackage = actionData.Value;

            ifNumberOfTargets.text = targetData.NumberOfTargets.ToString();
            ifMaxDistance.text = targetData.MaxDistance.ToString();
            ifMinDistance.text = targetData.MinDistance.ToString();
            ifRadius.text = targetData.Radius.ToString();

            targetTypeDropdown.value = supportedTargetTypes.LastIndexOf(targetData.TargetType);

            if (actionPackage.DamageOnFail.Count > 0)
            {
                DamageWidget damageWidget = Instantiate(damageWidgetPrefab, content.transform).GetComponent<DamageWidget>();
                damageWidget.Load(false, actionPackage.DamageOnFail);
            }
            if (actionPackage.DamageOnSuccess.Count > 0)
            {
                DamageWidget damageWidget = Instantiate(damageWidgetPrefab, content.transform).GetComponent<DamageWidget>();
                damageWidget.Load(true, actionPackage.DamageOnSuccess);
            }

            if (actionPackage.ModifiersOnFail.Count > 0)
            {
                AttributeWidget attributeWidget = Instantiate(modifierWidgetPrefab, content.transform).GetComponent<AttributeWidget>();
                attributeWidget.Load(false, actionPackage.ModifiersOnFail);
            }
            if (actionPackage.ModifiersOnSuccess.Count > 0)
            {
                AttributeWidget attributeWidget = Instantiate(modifierWidgetPrefab, content.transform).GetComponent<AttributeWidget>();
                attributeWidget.Load(true, actionPackage.ModifiersOnSuccess);
            }
        }
    }
}
