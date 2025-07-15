using Iterum.models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.ActionMaker
{
    public class DamageWidget : DraggableUI
    {
        public TMP_Dropdown successDropDown;
        public GameObject content;

        public GameObject damageEntryPrefab;

        public List<string> successOptions = new() { "Success", "Fail" };
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
            successDropDown.ClearOptions();
            successDropDown.AddOptions(successOptions);
            initialized = true;
        }

        public KeyValuePair<bool, List<DamageInfo>> GetDamage()
        {
            bool success = successDropDown.value == 0;
            List<DamageInfo> list = new();
            foreach (Transform child in content.transform)
            {
                if (child.TryGetComponent<DamageEntry>(out var widget))
                {
                    list.Add(widget.GetDamageEntry());
                }
            }
            return new KeyValuePair<bool, List<DamageInfo>>(success, list);
        }

        public void Load(bool onSuccess, IList<DamageInfo> damageInfos) {
            Init();

            if (onSuccess)
            {
                successDropDown.value = 0;
            }
            else
            {
                successDropDown.value = 1;
            }

            foreach (var damageInfo in damageInfos)
            {
                DamageEntry damageEntry = Instantiate(damageEntryPrefab, content.transform).GetComponent<DamageEntry>();
                damageEntry.Load(damageInfo);
            }
        }
    }
}
