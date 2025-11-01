using System;
using Iterum.Scripts.Map;
using System.Collections.Generic;
using Iterum.Scripts.Utils;
using Iterum.Scripts.Utils.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Iterum.DTOs;
using Unity.VisualScripting;

namespace Iterum.Scripts.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldAutoResize : MonoBehaviour
    {
        public float minHeight = 30f;
        public float padding = 10f;
        public TMP_Text lblRendered;

        private TMP_InputField input;
        public ScrollRect parentScrollRect;
        private RectTransform rect;
        

        void Start()
        {
            input = GetComponent<TMP_InputField>();
            rect = GetComponent<RectTransform>();
            //input.onValueChanged.AddListener(_ => Resize());
            input.onValueChanged.AddListener(s => RenderMarkdown(s));
            input.lineType = TMP_InputField.LineType.MultiLineNewline;
            input.scrollSensitivity = 0f;
            input.verticalScrollbar = null;
            Resize();
        }

        private void RenderMarkdown(string s)
        {
            lblRendered.text = MarkdownService.Convert(s);
        }

        void Resize()
        {
            float preferred = input.textComponent.preferredHeight + padding;
            float newHeight = Mathf.Max(minHeight, preferred);

            float currentScroll = 0f;
            if (parentScrollRect != null)
                currentScroll = parentScrollRect.verticalNormalizedPosition;

            // Only change height
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

            if (parentScrollRect != null)
                parentScrollRect.verticalNormalizedPosition = currentScroll;
        }
    }
}
