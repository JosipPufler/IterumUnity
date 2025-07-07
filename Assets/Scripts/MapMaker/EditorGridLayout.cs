using System.Collections.Generic;
using Assets.Scripts.Map;
using Iterum.Scripts.Map;
using Iterum.Scripts.UI;
using Iterum.Scripts.Utils.Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class EditorGridLayout : HexGridLayout
{
    public Material holderMaterial;

    private List<GameObject> hexHolders = new List<GameObject>();

    void OnEnable()
    {
        LayoutGrid();
        if (GameManager.Instance != null && GameManager.Instance.SelectedMap != null && GameManager.Instance.SelectedMap.Hexes != null)
        {
            foreach (Hex hex in GameManager.Instance.SelectedMap.Hexes)
            {
                TryAddHex(new Vector3Int(hex.X, hex.Y, hex.Z));
            }
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenu");
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
       Input.GetKeyDown(KeyCode.S))
        {
            List<Hex> hexes = new List<Hex>();
            foreach (var key in grid.Keys)
            {
                hexes.Add(new Hex(key.x, key.y, key.z));
            }
            GameManager.Instance.SelectedMap.Hexes = hexes;
            MapManager.Instance.UpdateMap(GameManager.Instance.SelectedMap, null, OnError);
        }

        // Place on top
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("Hex"))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f)
                    {
                        Debug.Log("Clicked on the top of the hex!");

                        foreach (var pair in grid)
                        {
                            if (pair.Value == hitObject)
                            {
                                Vector3Int currentKey = pair.Key;
                                Vector3Int aboveKey = new Vector3Int(currentKey.x, currentKey.y + 1, currentKey.z);
                                TryAddHex(aboveKey);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Vector3 localHit = hitObject.transform.InverseTransformPoint(hit.point);
                        localHit.y = 0f;

                        float rawAngle = Mathf.Atan2(localHit.z, localHit.x) * Mathf.Rad2Deg;
                        float angle360 = (rawAngle + 360f) % 360f;

                        int sector;
                        if (isFlatTopped)
                        {
                            sector = Mathf.FloorToInt((angle360 + 30f) / 60f) % 6;
                        }
                        else
                        {
                            sector = Mathf.FloorToInt((angle360 + 30f) / 60f) % 6;
                        }

                        Debug.Log(sector);

                        foreach (var pair in grid)
                        {
                            if (pair.Value == hitObject)
                            {
                                var currentKey = pair.Key;
                                int col = currentKey.x;
                                int row = currentKey.z;

                                int q = col;
                                int r = row - (col - (col & 1)) / 2;

                                Vector2Int dir = axialDirs[sector];
                                int q2 = q + dir.x;
                                int r2 = r + dir.y;

                                int col2 = q2;
                                int row2 = r2 + (q2 - (q2 & 1)) / 2;
                                Vector3Int neighborKey = new Vector3Int(col2, currentKey.y, row2);
                                TryAddHex(neighborKey);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Place
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                Vector3 worldPoint = hit.point;
                Vector2Int hexCoords = WorldToHex(worldPoint);

                if (hitObject.CompareTag("Ground"))
                {
                    Vector3 center = GetPositionForHexFromCoordinate(hexCoords);
                    TryAddBottomHex(hexCoords);
                }
                else if (hitObject.CompareTag("HexHolder"))
                {
                    TryAddBottomHex(hexCoords);
                }
            }
        }

        // Remove
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("Hex"))
                {
                    Vector3Int? keyToRemove = null;

                    foreach (var pair in grid)
                    {
                        if (pair.Value == hitObject)
                        {
                            keyToRemove = pair.Key;
                            break;
                        }
                    }

                    if (keyToRemove.HasValue)
                    {
                        Destroy(grid[keyToRemove.Value]);
                        grid.Remove(keyToRemove.Value);
                    }
                    else
                    {
                        Debug.LogWarning("Hex GameObject found, but not in dictionary.");
                    }
                }
            }
        }
    }

    protected void TryAddBottomHex(Vector2Int vector2) {
        Vector3Int key = new Vector3Int(vector2.x, 0, vector2.y);

        TryAddHex(key);
    }

    public void LayoutGrid()
    {
        foreach (var item in hexHolders)
        {
            Destroy(item);
        }
        hexHolders.Clear();

        for (int y = 0; y < gridSize.y; y++)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
                GameObject tile = new GameObject($"Hex {x},{y}", typeof(HexRenderer));
                tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));

                tile.tag = "HexHolder";

                HexRenderer r = tile.GetComponent<HexRenderer>();
                r.Initialize(holderMaterial, innerSize, outerSize, height, isFlatTopped);

                tile.transform.SetParent(transform, true);
                hexHolders.Add(tile);
            }
        }

        List<Vector3Int> keysToDelete = new List<Vector3Int>();
        foreach (var hex in grid.Keys)
        {
            if (hex.x >= gridSize.x || hex.z >= gridSize.y) {
                keysToDelete.Add(hex);
            }
        }

        foreach (var key in keysToDelete)
        {
            Destroy(grid[key]);
            grid.Remove(key);
        }
    }
}
