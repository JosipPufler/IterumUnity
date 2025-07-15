using Assets.Scripts.GameLogic.models.target;
using Iterum.models;
using Iterum.models.enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.ActionMaker
{
    public class AttributeWidget : DraggableUI
    {
        public TMP_Dropdown successDropDown;
        public GameObject content;

        public GameObject attributeEntryPrefab;

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

        public KeyValuePair<bool, Dictionary<Attribute, int>> GetAttributes() {
            bool success = successDropDown.value == 0;
            Dictionary<Attribute, int> dictionary = new();
            foreach (Transform child in content.transform)
            {
                if (child.TryGetComponent<AttributeEntry>(out var widget))
                {
                    KeyValuePair<Attribute, int> keyValuePair = widget.GetAttributeEntry();
                    dictionary[keyValuePair.Key] = dictionary.GetValueOrDefault(keyValuePair.Key) + keyValuePair.Value;
                }
            }
            return new KeyValuePair<bool, Dictionary<Attribute, int>>(success, dictionary);
        }

        public void Load(bool onSuccess, Dictionary<Attribute, int> modifiers) {
            Init();
            if (onSuccess)
            {
                successDropDown.value = 0;
            }
            else
            {
                successDropDown.value = 1;
            }

            foreach (var modifier in modifiers)
            {
                AttributeEntry attributeEntry = Instantiate(attributeEntryPrefab, content.transform).GetComponent<AttributeEntry>();
                attributeEntry.Load(modifier);
            }
        }
    }
}
