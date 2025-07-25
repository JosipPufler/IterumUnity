namespace Assets.Scripts.GameLogic.models.target
{
    public class TargetDataSubmissionHex : TargetDataSubmission
    {
        public TargetDataSubmissionHex() { }
        public TargetDataSubmissionHex(TargetData targetData, GridCoordinate targetable) : base(targetData){
            GridCoordinate = targetable;
        }

        public GridCoordinate GridCoordinate;

        public override object GetTargetable() => GridCoordinate;
    }
}
