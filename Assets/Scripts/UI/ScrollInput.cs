using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ScrollInput : MonoBehaviour
    {
        public TMP_InputField inputField;
        public ScrollRect scrollRect;

        private void Start()
        {
            //inputField.onValueChanged.AddListener(OnInputValueChanged);
        }

        private void OnInputValueChanged(string _)
        {
            StartCoroutine(RestoreScrollPosition());
        }

        private System.Collections.IEnumerator RestoreScrollPosition()
        {
            float scrollY = scrollRect.verticalNormalizedPosition;

            yield return null;

            scrollRect.verticalNormalizedPosition = scrollY;
        }
    }
}
