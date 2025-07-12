namespace Assets.Scripts.GameLogic.models
{
    public abstract class TargetDataSubmission
    {
        public TargetDataSubmission(TargetData targetData, object targetable)
        {
            TargetData = targetData;
            Targetable = targetable;
        }

        public TargetData TargetData { get; set; }
        public virtual object Targetable { get; set; }
    }
}
