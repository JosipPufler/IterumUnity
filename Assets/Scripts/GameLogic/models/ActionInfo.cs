using Assets.Scripts.GameLogic.models;
using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Iterum.models
{
    public class ActionInfo
    {
        public BaseCreature OriginCreature { get; set; }
        public Dictionary<TargetData, List<TargetDataSubmission>> Targets { get; set; } = new();
    }
}
