using System;
using System.Collections.Generic;
using Iterum.Scripts.Map;

namespace Iterum.DTOs
{
    [Serializable]
    public class MapDto
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public List<Hex> Hexes { get; set; }
        public bool IsFlatTopped;
        public int maxX;
        public int maxY;

        public MapDto()
        {
            
        }

        public MapDto(string name, List<Hex> hexs, string id, bool isFlatTopped)
        {
            Name = name;
            Hexes = hexs;
            Id = id;
            IsFlatTopped = isFlatTopped;
        }

        public MapDto(string name, List<Hex> hexs, bool isFlatTopped)
        {
            Name = name;
            Hexes = hexs;
            IsFlatTopped = isFlatTopped;
        }
    }
}
