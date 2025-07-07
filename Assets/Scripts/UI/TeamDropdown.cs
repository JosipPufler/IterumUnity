using Assets.Scripts.GameLogic.models.enums;
using Iterum.Scripts.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TeamDropdown : MonoBehaviour
    {
        public TMP_Dropdown dropdown;

        void Start()
        {
            dropdown.ClearOptions();

            var options = new List<string>(System.Enum.GetNames(typeof(Team)));
            dropdown.AddOptions(options);
            dropdown.onValueChanged.AddListener(value => {
                GameManager.Instance.Team = (Team)value;
            });
        }
    }
}
