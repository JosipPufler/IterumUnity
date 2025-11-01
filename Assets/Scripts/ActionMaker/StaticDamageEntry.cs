using Iterum.models;
using Iterum.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.ActionMaker
{
    public class StaticDamageEntry : MonoBehaviour
    {
        public TMP_Dropdown damageTypeDropdown;
        public TMP_InputField numberOfDice;
        public TMP_Dropdown diceDropDown;

        bool initialized = false;

        private void Start()
        {
            if (!initialized)
            {
                Init();
            }
        }

        private void Init()
        {
            damageTypeDropdown.ClearOptions();
            List<string> damageTypeNames = DamageType.GetDamageTypes().Select(x => x.Name).ToList();
            damageTypeDropdown.AddOptions(damageTypeNames);

            diceDropDown.ClearOptions();
            List<string> dieNames = Enum.GetNames(typeof(Dice)).ToList();
            diceDropDown.AddOptions(dieNames);
            initialized = true;
        }

        public DamageInfo? GetDamageEntry()
        {
            if (int.TryParse(numberOfDice.text, out int value))
                return new DamageInfo(value, Enum.GetValues(typeof(Dice)).Cast<Dice>().ToArray()[diceDropDown.value], DamageType.GetDamageTypes().ElementAt(damageTypeDropdown.value));
            return null;
        }

        public void Load(DamageInfo damageInfo)
        {
            Init();
            numberOfDice.text = damageInfo.NumberOfDice.ToString();
            diceDropDown.value = Enum.GetValues(typeof(Dice)).Cast<Dice>().ToList().IndexOf(damageInfo.Die);
            damageTypeDropdown.value = DamageType.GetDamageTypes().IndexOf(damageInfo.DamageType);
        }
    }
}
