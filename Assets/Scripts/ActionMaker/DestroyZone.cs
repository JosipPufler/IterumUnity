using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ActionMaker
{
    public class DestroyZone : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            var draggable = eventData.pointerDrag?.GetComponent<DraggableUI>();
            if (draggable != null)
            {
                Destroy(draggable.gameObject);
            }
        }
    }
}
