using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIToolTipTrigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
    {
        public string tooltipText;

        public void OnPointerEnter(PointerEventData eventData)
        {
            ToolTipManager.Instance.Show(tooltipText);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToolTipManager.Instance.Hide();
        }
    }
}
