using System;

namespace Iterum.Scripts.Map
{
    [Serializable]
    public struct Hex
    {
        public Hex(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X;
        public int Y;
        public int Z;
    }
}
