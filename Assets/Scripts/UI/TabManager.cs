using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TabManager : MonoBehaviour
    {
        [Serializable]
        public class Tab
        {
            public Button tabButton;
            public GameObject contentPanel;
        }

        public List<Tab> tabs;

        void Start()
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                int index = i;
                tabs[i].tabButton.onClick.AddListener(() => OpenTab(index));
            }

            OpenTab(0);
        }

        public void OpenTab(int index)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                bool isActive = i == index;
                tabs[i].contentPanel.SetActive(isActive);
            }
        }
    }
}
