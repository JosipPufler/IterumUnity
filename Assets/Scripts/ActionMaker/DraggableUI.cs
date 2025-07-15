using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ActionMaker
{
    public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        RectTransform rect;
        CanvasGroup canvasGroup;
        Transform originalParent;
        Vector2 originalPosition;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            if (!gameObject.TryGetComponent<CanvasGroup>(out CanvasGroup component))
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            else
            {
                canvasGroup = component;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            originalParent = rect.parent;
            originalPosition = rect.localPosition;

            canvasGroup.blocksRaycasts = false;
            rect.SetParent(originalParent.root, true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            rect.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;

            if (rect.parent == originalParent.root)
            {
                rect.SetParent(originalParent, true);
                rect.localPosition = originalPosition;
            }
        }

        public void SetParent(Transform newParent)
        {
            rect.SetParent(newParent, true);
        }
    }
}
