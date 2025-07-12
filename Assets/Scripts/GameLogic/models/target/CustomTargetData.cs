using Assets.Scripts.GameLogic.models.enums;
using Iterum.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameLogic.models.target
{
    public class CustomTargetData : TargetData
    {
        public CustomTargetData(TargetType targetType, int minDistance, int maxDistance, ActionType actionType, Stat statUsed, int radius, AttackType attackType = null) : base(targetType, minDistance, maxDistance)
        {
            Stat = statUsed;
            ActionType = actionType;
            if (actionType == ActionType.Attack && attackType != null) {
                AttackType = attackType;
            }
            if (targetType == TargetType.Tile) { 
                Radius = radius;
            }
        }

        public AttackType AttackType { get; set; }
        public Stat Stat { get; set; }
        public int Radius { get; set; }
        public ActionType ActionType { get; set; }
    }
}
