using Newtonsoft.Json;

namespace Assets.Scripts.GameLogic.models
{
    public abstract class TargetDataSubmission
    {
        protected TargetDataSubmission(){}

        public TargetDataSubmission(TargetData targetData)
        {
            TargetData = targetData;
        }

        public TargetData TargetData { get; set; }
        public abstract object GetTargetable();
    }
}
