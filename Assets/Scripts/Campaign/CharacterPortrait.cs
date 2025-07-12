// Assets/Scripts/Utils/CharacterPortrait.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Campaign
{
    public class CharacterPortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public CharacterToken CharacterToken;
        public CameraController CameraRig;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => CameraRig.FocusOn(CharacterToken.transform.position));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CharacterToken.forceOutline = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CharacterToken.forceOutline = false;
        }
    }
}
