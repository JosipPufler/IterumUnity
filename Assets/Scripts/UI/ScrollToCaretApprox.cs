using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Iterum.Scripts.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class ScrollToCaretApprox : MonoBehaviour
    {
        public ScrollRect parentScrollRect;
        private TMP_InputField input;

        void Awake()
        {
            input = GetComponent<TMP_InputField>();
            //input.onValueChanged.AddListener(_ => ScrollApprox());
            input.onSelect.AddListener(_ => ScrollApprox());
        }

        void ScrollApprox()
        {
            Canvas.ForceUpdateCanvases();
            int caretIndex = input.caretPosition;
            int total = input.text.Length > 0 ? input.text.Length : 1;

            // Invert because 1 = top, 0 = bottom
            float norm = 1f - ((float)caretIndex / total);
            parentScrollRect.verticalNormalizedPosition = Mathf.Clamp01(norm);
        }
    }
}
