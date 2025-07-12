using UnityEngine;

namespace Assets.Scripts.GameLogic.models.target
{
    public class TargetDataSubmissionHex : TargetDataSubmission
    {
        public TargetDataSubmissionHex(TargetData targetData, Vector3Int targetable) : base(targetData, targetable)
        {
        }

        public new Vector3Int Targetable => (Vector3Int)base.Targetable;
    }
}
