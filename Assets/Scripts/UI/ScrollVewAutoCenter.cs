using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewAutoCenter : MonoBehaviour
{
    public RectTransform content;
    public RectTransform viewport;
    public ScrollRect scrollRect;

    void Start()
    {
        UpdateScroll();
    }

    public void UpdateScroll()
    {
        float totalPreferredWidth = 0f;

        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform child = content.GetChild(i) as RectTransform;
            if (child != null)
            {
                totalPreferredWidth += LayoutUtility.GetPreferredWidth(child);
            }
        }

        float spacing = 0f;
        HorizontalLayoutGroup hlg = content.GetComponent<HorizontalLayoutGroup>();
        if (hlg != null)
        {
            spacing = hlg.spacing * (content.childCount - 1);
            totalPreferredWidth += hlg.padding.left + hlg.padding.right + spacing;
        }

        float viewportWidth = viewport.rect.width;

        if (totalPreferredWidth <= viewportWidth)
        {
            // Disable scrolling and center
            scrollRect.horizontal = false;

            float centeredX = (viewportWidth - totalPreferredWidth) / 2f;
            content.anchoredPosition = new Vector2(centeredX, content.anchoredPosition.y);
        }
        else
        {
            // Enable scrolling and snap to left
            scrollRect.horizontal = true;
            content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y);
        }
    }

    void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        UpdateScroll();
    }
}
