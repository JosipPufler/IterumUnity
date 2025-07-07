using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Iterum.Scripts.UI
{
    public class ForwardScrollEvents : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] ScrollRect scrollRect;

        public void OnBeginDrag(PointerEventData e) => scrollRect.OnBeginDrag(e);
        public void OnDrag(PointerEventData e) => scrollRect.OnDrag(e);
        public void OnEndDrag(PointerEventData e) => scrollRect.OnEndDrag(e);
    }
}
