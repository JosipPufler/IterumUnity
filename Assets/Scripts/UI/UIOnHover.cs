using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

namespace Assets.Scripts.UI
{
    public class UIOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Action onHoverEnter;
        public Action onHoverExit;

        public void OnPointerEnter(PointerEventData eventData)
        {
            onHoverEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHoverExit?.Invoke();
        }
    }
}
