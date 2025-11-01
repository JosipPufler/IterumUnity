using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Iterum.Scripts.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class ScrollToCaretInParent : MonoBehaviour
    {
        public ScrollRect parentScrollRect;
        public float caretOffset = 0;

        private TMP_InputField input;

        void Awake()
        {
            input = GetComponent<TMP_InputField>();
            input.onValueChanged.AddListener(_ => ScrollToCaret());
            input.onSelect.AddListener(_ => ScrollToCaret());
        }

        void ScrollToCaret()
        {
            StartCoroutine(ScrollNextFrame());
        }

        System.Collections.IEnumerator ScrollNextFrame()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();

            int caretIndex = input.caretPosition;
            var textInfo = input.textComponent.textInfo;

            if (textInfo.characterCount == 0 || caretIndex >= textInfo.characterCount)
                yield break;

            var charInfo = textInfo.characterInfo[caretIndex];
            Vector3 worldPos = input.textComponent.transform.TransformPoint(charInfo.bottomLeft);

            RectTransform contentRT = parentScrollRect.content;
            Vector3 localPos = contentRT.InverseTransformPoint(worldPos);

            float contentHeight = contentRT.rect.height;
            float localCaretY = localPos.y;

            float viewportHeight = parentScrollRect.viewport.rect.height;
            float targetScrollY = Mathf.Clamp01(1 - ((localCaretY + caretOffset) / (contentHeight - viewportHeight)));

            parentScrollRect.verticalNormalizedPosition = targetScrollY;
        }
    }
}
