using UnityEngine;
using UnityEngine.EventSystems;

public class VerticalResizeHandler : MonoBehaviour, IDragHandler
{
    public RectTransform panelToResize;

    public void OnDrag(PointerEventData eventData)
    {
        if (panelToResize == null) return;

        Vector2 newSize = panelToResize.sizeDelta + new Vector2(0, eventData.delta.y);

        if (newSize.y <= 20 || newSize.y > 900)
        {
            return;
        }

        panelToResize.sizeDelta = newSize;
    }
}
