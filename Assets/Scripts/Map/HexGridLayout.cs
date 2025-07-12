using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public abstract class HexGridLayout : MonoBehaviour
    {
        [Header("Grid settings")]
        public Vector2Int gridSize;

        [Header("Tile settings")]
        public float outerSize = 100f;
        public float innerSize = 90;
        public float height = 50f;
        public bool isFlatTopped = true;
        public Material hexMaterial;
        public Material highlightMaterial;

        protected static readonly Dictionary<Vector3Int, GameObject> grid = new();
        protected static readonly Vector2Int[] axialDirs = {
            new(+1,  0), new(+1, -1), new( 0, -1),
            new(-1,  0), new(-1, +1), new( 0, +1)
        };


        protected virtual void TryAddHex(Vector3Int key)
        {
            if (key.z > gridSize.y - 1 || key.z < 0 || key.x > gridSize.x - 1 || key.x < 0)
            {
                return;
            }

            if (!grid.TryGetValue(key, out GameObject hex))
            {
                GameObject tile = new($"Hex {key.x},{key.y}, ", typeof(HexRenderer));
                tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(key.x, key.z));
                tile.tag = "Hex";

                HexRenderer r = tile.GetComponent<HexRenderer>();
                r.Initialize(hexMaterial, highlightMaterial, 0, innerSize, height * 2, isFlatTopped, key.y * height * 2);

                tile.transform.SetParent(transform, true);
                grid[key] = tile;
            }
        }

        protected Vector3 GetPositionForHexFromCoordinate(Vector2Int vector2Int)
        {
            int column = vector2Int.x;
            int row = vector2Int.y;
            float width;
            float height;
            float xPosition;
            float yPosition;
            bool shouldOffset;
            float horizontalDistance;
            float verticalDistance;
            float offset;
            float size = outerSize;


            if (!isFlatTopped)
            {
                shouldOffset = (row % 2) == 0;
                width = Mathf.Sqrt(3) * size;
                height = 2f * size;

                horizontalDistance = width;
                verticalDistance = height * 3f / 4f;

                offset = shouldOffset ? width / 2 : 0;

                xPosition = (column * horizontalDistance) + offset;
                yPosition = (row * verticalDistance);
            }
            else
            {
                shouldOffset = (column % 2) == 0;
                width = 2f * size;
                height = Mathf.Sqrt(3f) * size;

                horizontalDistance = width * 3f / 4f;
                verticalDistance = height;

                offset = shouldOffset ? height / 2 : 0;

                xPosition = column * horizontalDistance;
                yPosition = (row * verticalDistance) - offset;
            }

            return new Vector3(xPosition, 0, -yPosition);
        }

        private Vector2Int OffsetToAxial(Vector2Int offset)
        {
            int q, r;

            if (isFlatTopped)
            {
                q = offset.x;
                r = offset.y - (offset.x >> 1);
            }
            else
            {
                q = offset.x - (offset.y >> 1);
                r = offset.y;
            }
            return new Vector2Int(q, r);
        }

        public bool AreAdjacent(Vector3Int a, Vector3Int b)
        {
            if (Mathf.Abs(a.y - b.y) > 1) return false;

            Vector2Int axialA = OffsetToAxial(new Vector2Int(a.x, a.z));
            Vector2Int axialB = OffsetToAxial(new Vector2Int(b.x, b.z));
            Vector2Int delta = axialB - axialA;

            foreach (var dir in axialDirs)
                if (delta == dir) return true;

            return false;
        }

        public Vector2Int WorldToHex(Vector3 worldPos)
        {
            float size = outerSize;

            if (isFlatTopped)
            {
                float width = 2f * size;
                float height = Mathf.Sqrt(3f) * size;
                float horiz = 0.75f * width;
                float vert = height;

                float x = worldPos.x;
                float y = -worldPos.z;

                int q = Mathf.RoundToInt(x / horiz);
                float offset = (q % 2 == 0) ? vert / 2f : 0f;
                int r = Mathf.RoundToInt((y + offset) / vert);

                return new Vector2Int(q, r);
            }
            else
            {
                float height = 2f * size;
                float width = Mathf.Sqrt(3f) * size;
                float horiz = width;
                float vert = 0.75f * height;

                float x = worldPos.x;
                float y = -worldPos.z;

                int r = Mathf.RoundToInt(y / vert);
                float offset = (r % 2 == 0) ? horiz / 2f : 0f;
                int q = Mathf.RoundToInt((x - offset) / horiz);

                return new Vector2Int(q, r);
            }
        }

        protected void OnError(string error)
        {
            Debug.LogError(error);
        }
    }
}
