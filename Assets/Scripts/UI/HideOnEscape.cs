using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HideOnEscape : MonoBehaviour
    {
        public GameObject objectToHide;
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                objectToHide.SetActive(false);
            }
        }
    }
}
