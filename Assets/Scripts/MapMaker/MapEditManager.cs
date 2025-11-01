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
            if (gridSize.x <= 0)
            {
                gridSize.x = 10;
            }
            if (gridSize.y <= 0)
            {
                gridSize.y = 10;
            }
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
            if (GameManager.Instance != null && GameManager.Instance.SelectedMap.IsValid() && GameManager.Instance.SelectedMap.Hexes != null)
            {
                ifX.text = GameManager.Instance.SelectedMap.MaxX.ToString();
                ifY.text = GameManager.Instance.SelectedMap.MaxY.ToString();
                gridSize.x = GameManager.Instance.SelectedMap.MaxX;
                gridSize.y = GameManager.Instance.SelectedMap.MaxY;
                LayoutGrid();
                foreach (Hex hex in GameManager.Instance.SelectedMap.Hexes)
                {
                    TryAddHex(new GridCoordinate(hex.X, hex.Y, hex.Z));
                }
            }
        }
    }
}