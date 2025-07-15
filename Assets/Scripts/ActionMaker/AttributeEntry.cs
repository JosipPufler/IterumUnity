using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Attribute = Iterum.models.enums.Attribute;

namespace Assets.Scripts.ActionMaker
{
    public class AttributeEntry : DraggableUI
    {
        public TMP_Dropdown attributeDropdown;
        public TMP_InputField inputField;

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
            attributeDropdown.ClearOptions();
            List<string> attributeNames = Enum.GetNames(typeof(Attribute)).ToList();
            attributeDropdown.AddOptions(attributeNames);
            initialized = true;
        }

        public KeyValuePair<Attribute, int> GetAttributeEntry()
        {
            return new KeyValuePair<Attribute, int>((Attribute)attributeDropdown.value, int.Parse(inputField.text));
        }

        public void Load(KeyValuePair<Attribute, int> modifier) {
            Init();
            attributeDropdown.value = Enum.GetNames(typeof(Attribute)).ToList().IndexOf(modifier.Key.ToString());
            inputField.text = modifier.Key.ToString();
        }
    }
}
