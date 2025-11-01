using Assets.Scripts.GameLogic.models.enums;
using Iterum.models.enums;

namespace Assets.Scripts.GameLogic.models.target
{
    public class CustomTargetData : TargetData
    {
        public CustomTargetData() : base(TargetType.Creature, 0, 0) { }

        public CustomTargetData(TargetType targetType, int minDistance, int maxDistance, int variable) : base(targetType, minDistance, maxDistance)
        {
            ActionType = ActionType.Command;
            if (targetType == TargetType.Creature)
            {
                NumberOfTargets = variable;
            }
            else if (targetType == TargetType.Tile)
            {
                Radius = variable;
                NumberOfTargets = 1;
            }
        }

        public CustomTargetData(TargetType targetType, int minDistance, int maxDistance, int variable, AttackType attackType) : base(targetType, minDistance, maxDistance)
        {
            AttackType = attackType;
            ActionType = ActionType.Attack;
            ActionType = ActionType.Command;
            if (targetType == TargetType.Creature)
            {
                NumberOfTargets = variable;
            }
            else if (targetType == TargetType.Tile)
            {
                Radius = variable;
                NumberOfTargets = 1;
            }
        }

        public CustomTargetData(TargetType targetType, int minDistance, int maxDistance, int variable, SavingThrow savingThrow) : base(targetType, minDistance, maxDistance)
        {
            SavingThrow = savingThrow;
            ActionType = ActionType.SavingThrow;
            if (targetType == TargetType.Creature)
            {
                NumberOfTargets = variable;
            }
            else if (targetType == TargetType.Tile)
            {
                Radius = variable;
                NumberOfTargets = 1;
            }
        }

        public AttackType AttackType { get; set; }
        public SavingThrow SavingThrow { get; set; }
        public int Radius { get; set; }
        public int NumberOfTargets { get; set; }
        public ActionType ActionType { get; set; }
    }
}
