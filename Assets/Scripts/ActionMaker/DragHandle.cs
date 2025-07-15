using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ActionMaker
{
    [RequireComponent(typeof(RectTransform))]
    public class DragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        RectTransform rectTransform;
        Vector2 offset;

        void Awake() => rectTransform = GetComponent<RectTransform>();

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out offset);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform,
                eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            {
                rectTransform.localPosition = localPoint - offset;
            }
        }
    }
}
