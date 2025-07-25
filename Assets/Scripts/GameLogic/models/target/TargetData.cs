using Iterum.models.enums;
using System;

namespace Assets.Scripts.GameLogic.models
{
    public class TargetData
    {
        public TargetData()
        {
            
        }

        public TargetData(TargetType targetType, int minDistance, int maxDistance)
        {
            TargetType = targetType;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
        }

        public string ID { get; set; } = Guid.NewGuid().ToString();
        public TargetType TargetType { get; set; }
        public int MinDistance { get; set; }
        public int MaxDistance { get; set; }

        public override bool Equals(object obj)
        {
            return obj is TargetData data &&
                   TargetType == data.TargetType &&
                   MinDistance == data.MinDistance &&
                   MaxDistance == data.MaxDistance;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TargetType, MinDistance, MaxDistance);
        }
    }
}
