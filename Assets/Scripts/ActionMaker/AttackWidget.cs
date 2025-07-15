using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Iterum.models.enums;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.UI;

namespace Assets.Scripts.ActionMaker
{
    public class AttackWidget : MakerWidget
    {
        public TMP_Dropdown attackTypeDropDown;
        public TMP_Dropdown attackStatDropDown;
        public Toggle proficiency;

        readonly List<AttackTypeEnum> supportedAttackTypeList = new() { AttackTypeEnum.MeleeWeapon, AttackTypeEnum.RangedWeapon, AttackTypeEnum.Spell};

        public override CustomTargetData GetCustomTargetData()
        {
            AttackType attackType = AttackType.CreateByEnum(supportedAttackTypeList[attackStatDropDown.value], Stat.GetAllStats().ElementAt(attackStatDropDown.value), proficiency.isOn);
            if (supportedTargetTypes[targetTypeDropdown.value] == TargetType.Tile)
            {
                return new CustomTargetData(supportedTargetTypes[targetTypeDropdown.value], int.Parse(ifMinDistance.text), int.Parse(ifMaxDistance.text), int.Parse(ifRadius.text), attackType);
            }
            else
            {
                return new CustomTargetData(supportedTargetTypes[targetTypeDropdown.value], int.Parse(ifMinDistance.text), int.Parse(ifMaxDistance.text), int.Parse(ifNumberOfTargets.text), attackType);
            }
        }

        protected override void Init() { 
            base.Init();

            attackTypeDropDown.ClearOptions();
            var attackTypeNames = supportedAttackTypeList.Select(t => t.ToString()).ToList();
            attackTypeDropDown.AddOptions(attackTypeNames);

            attackStatDropDown.ClearOptions();
            var statNames = Stat.GetAllStats().Select(t => t.Name).ToList();
            attackStatDropDown.AddOptions(statNames);
        }

        public override void LoadAction(KeyValuePair<CustomTargetData, ActionPackage> actionData)
        {
            base.LoadAction(actionData);

            AttackType attackType = actionData.Key.AttackType;

            attackStatDropDown.value = Stat.GetAllStats().ToList().IndexOf(attackType.BaseAttribute);
            attackTypeDropDown.value = supportedAttackTypeList.IndexOf(attackType.AttackTypeEnum);
            proficiency.isOn = attackType.Proficient;
        }
    }
}
