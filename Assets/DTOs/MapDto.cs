using System;
using System.Collections.Generic;
using Iterum.Scripts.Map;

namespace Iterum.DTOs
{
    [Serializable]
    public struct MapDto
    {
        public string? Id;
        public string Name;
        public List<Hex> Hexes;
        public bool IsFlatTopped;
        public int MaxX;
        public int MaxY;

        public readonly bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && Hexes != null && Hexes.Count > 0 && MaxX > 0 && MaxY > 0;
        }
    }
}
