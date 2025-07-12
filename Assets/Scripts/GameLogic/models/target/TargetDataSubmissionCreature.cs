using Iterum.models.interfaces;

namespace Assets.Scripts.GameLogic.models.target
{
    public class TargetDataSubmissionCreature : TargetDataSubmission
    {
        public TargetDataSubmissionCreature(TargetData targetData, ICreature targetable) : base(targetData, targetable)
        {
        }

        public new ICreature Targetable => (ICreature)base.Targetable;
    }
}
