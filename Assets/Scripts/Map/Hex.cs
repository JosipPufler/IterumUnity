﻿using System;

namespace Iterum.Scripts.Map
{
    [Serializable]
    public class Hex
    {
        public Hex(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
