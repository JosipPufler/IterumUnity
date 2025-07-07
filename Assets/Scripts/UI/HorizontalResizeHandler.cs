using UnityEngine;
using UnityEngine.EventSystems;

public class HorizontalResizeHandler : MonoBehaviour, IDragHandler
{
    public RectTransform leftPanel;
    public RectTransform rightPanel;
    public RectTransform splitter;
    public float minWidth = 100f;

    public void OnDrag(PointerEventData eventData)
    {
        float delta = eventData.delta.x;

        float newLeftWidth = leftPanel.sizeDelta.x + delta;
        float newRightWidth = rightPanel.sizeDelta.x - delta;

        if (newLeftWidth < minWidth || newRightWidth < minWidth)
            return;

        leftPanel.sizeDelta = new Vector2(newLeftWidth, leftPanel.sizeDelta.y);
        rightPanel.sizeDelta = new Vector2(newRightWidth, rightPanel.sizeDelta.y);

        Vector2 splitterPos = splitter.anchoredPosition;
        splitterPos.x += delta;
        splitter.anchoredPosition = splitterPos;
    }
}
