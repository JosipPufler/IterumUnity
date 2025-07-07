using System.Collections.Generic;
using UnityEngine;

namespace Iterum.models.interfaces
{
    public interface IGameObject : IGameEntity
    {
        Vector3Int CurrentPosition { get; set; }
    }
}
