using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Iterum.models.enums;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.ActionMaker
{
    public class SavingThrowWidget : MakerWidget
    {
        public TMP_Dropdown originStatDropdown;
        public TMP_Dropdown saveStatDropdown;

        List<Stat> stats;

        public override CustomTargetData GetCustomTargetData()
        {
            SavingThrow savingThrow = new(stats.ElementAt(originStatDropdown.value), stats.ElementAt(saveStatDropdown.value));
            if (supportedTargetTypes[targetTypeDropdown.value] == TargetType.Tile)
            {
                return new CustomTargetData(supportedTargetTypes[targetTypeDropdown.value], int.Parse(ifMinDistance.text), int.Parse(ifMaxDistance.text), int.Parse(ifRadius.text), savingThrow);
            }
            else
            {
                return new CustomTargetData(supportedTargetTypes[targetTypeDropdown.value], int.Parse(ifMinDistance.text), int.Parse(ifMaxDistance.text), int.Parse(ifNumberOfTargets.text), savingThrow);
            }
        }

        protected override void Init()
        {
            base.Init();
            stats = Stat.GetAllStats().ToList();

            originStatDropdown.ClearOptions();
            saveStatDropdown.ClearOptions();
            var statNames = stats.Select(t => t.Name).ToList();
            originStatDropdown.AddOptions(statNames);
            saveStatDropdown.AddOptions(statNames);
        }

        public override void LoadAction(KeyValuePair<CustomTargetData, ActionPackage> actionData)
        {
            base.LoadAction(actionData);

            SavingThrow savingThrow = actionData.Key.SavingThrow;

            originStatDropdown.value = stats.IndexOf(savingThrow.OriginStat);
            saveStatDropdown.value = stats.IndexOf(savingThrow.SaveStat);
        }
    }
}
