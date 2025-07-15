using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.ActionMaker
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        [Tooltip("Only allow objects with at least one of these components")]
        public List<TypeReference> allowedTypes;

        public void OnDrop(PointerEventData eventData)
        {
            var draggedObject = eventData.pointerDrag;
            if (draggedObject == null) return;

            if (!draggedObject.TryGetComponent<DraggableUI>(out var draggable)) return;

            bool allowed = false;

            foreach (var typeRef in allowedTypes)
            {
                if (typeRef.Type != null && draggedObject.GetComponent(typeRef.Type) != null)
                {
                    allowed = true;
                    break;
                }
            }

            if (allowed)
            {
                draggable.SetParent(transform);
                StartCoroutine(DelayedLayoutRebuild());
            }
        }
        private IEnumerator DelayedLayoutRebuild()
        {
            yield return new WaitForEndOfFrame();

            Transform current = transform;
            while (current != null)
            {
                var layoutGroup = current.GetComponent<VerticalLayoutGroup>();
                if (layoutGroup != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(current as RectTransform);
                }
                current = current.parent;
            }
        }
    }

    [Serializable]
    public class TypeReference
    {
        [SerializeField] private string typeName;

        public Type Type => string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName);

        public TypeReference(Type type)
        {
            typeName = type.AssemblyQualifiedName;
        }

        public string TypeName => typeName;
    }
}
