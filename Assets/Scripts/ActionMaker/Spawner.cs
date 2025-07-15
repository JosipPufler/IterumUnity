using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ActionMaker
{
    public class Spawner : MonoBehaviour
    {
        public GameObject parent;
        public GameObject prefab;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(Spawn);
        }

        private void Spawn()
        {
            GameObject instance = Instantiate(prefab, parent.transform);

            if (instance.TryGetComponent<RectTransform>(out var rectTransform))
            {
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 0);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}
