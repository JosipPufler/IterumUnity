using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [ExecuteAlways]
    public class FlowLayoutGroup : LayoutGroup
    {
        public float spacingX = 10f;
        public float spacingY = 10f;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            SetLayout();
        }

        public override void CalculateLayoutInputVertical()
        {
            SetLayout();
        }

        public override void SetLayoutHorizontal() { }
        public override void SetLayoutVertical() { }

        private void SetLayout()
        {
            float maxWidth = rectTransform.rect.width;
            float x = padding.left;
            float y = -padding.top;
            float rowHeight = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float width = LayoutUtility.GetPreferredWidth(child);
                float height = LayoutUtility.GetPreferredHeight(child);

                if (x + width + padding.right > maxWidth)
                {
                    // Move to new row
                    x = padding.left;
                    y -= rowHeight + spacingY;
                    rowHeight = 0;
                }

                SetChildAlongAxis(child, 0, x, width);
                SetChildAlongAxis(child, 1, y, height);

                x += width + spacingX;
                rowHeight = Mathf.Max(rowHeight, height);
            }

            float totalHeight = Mathf.Abs(y) + rowHeight + padding.bottom;
            SetLayoutInputForAxis(totalHeight, totalHeight, -1, 1);
        }
    }
}
