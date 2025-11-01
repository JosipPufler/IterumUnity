using UnityEngine;

namespace Assets.Scripts
{
    public struct GridCoordinate
    {
        public int x;
        public int y;
        public int z;

        public GridCoordinate(Vector3Int vector3) { 
            x = vector3.x; 
            y = vector3.y; 
            z = vector3.z;
        }
        public GridCoordinate(Vector3 vector3)
        {
            x = (int)vector3.x;
            y = (int)vector3.y;
            z = (int)vector3.z;
        }
        public GridCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public readonly Vector3Int ToVector() { 
            return new Vector3Int(x, y, z);
        }

        public readonly override string ToString()
        {
            return $"{x}, {y}, {z}";
        }
    }
}
