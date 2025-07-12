using Iterum.Scripts.Map;
using Iterum.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Map
{
    public class MapEditManager : EditorGridLayout
    {
        [Header("UI")]
        public TMP_InputField ifX;
        public TMP_InputField ifY;
        public Toggle isFlatToppedToggle;

        void OnEnable()
        {
            ifX.text = gridSize.x.ToString();
            ifY.text = gridSize.y.ToString();
            if (!isFlatTopped)
            {
                isFlatToppedToggle.isOn = false;
            }
            isFlatToppedToggle.onValueChanged.AddListener(x =>
            {
                isFlatTopped = x;
                LayoutGrid();
            });

            ifX.onEndEdit.AddListener(x =>
            {
                if (!int.TryParse(x, out int result) || result <= 0)
                {
                    ifX.text = gridSize.x.ToString();
                    return;
                }

                gridSize.x = result;
                LayoutGrid();
            });

            ifY.onEndEdit.AddListener(y =>
            {
                if (!int.TryParse(y, out int result) || result <= 0)
                {
                    ifY.text = gridSize.y.ToString();
                    return;
                }

                gridSize.y = result;
                LayoutGrid();
            });

            LayoutGrid();
            if (GameManager.Instance != null && GameManager.Instance.SelectedMap != null && GameManager.Instance.SelectedMap.Hexes != null)
            {
                foreach (Hex hex in GameManager.Instance.SelectedMap.Hexes)
                {
                    TryAddHex(new Vector3Int(hex.X, hex.Y, hex.Z));
                }
            }
        }
    }
}