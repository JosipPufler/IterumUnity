using Mirror;
using Newtonsoft.Json;

namespace Assets.Scripts.GameLogic.models.target
{
    public class TargetDataSubmissionCreature : TargetDataSubmission
    {
        public TargetDataSubmissionCreature(){}
        public TargetDataSubmissionCreature(TargetData targetData, uint targetable) : base(targetData){
            TargetableNetId= targetable;
        }

        public uint TargetableNetId { get; set; }

        public override object GetTargetable() => TargetableNetId;

        public CharacterToken GetToken() {
            if (NetworkServer.spawned.TryGetValue(TargetableNetId, out NetworkIdentity identity))
            {
                return identity.gameObject.GetComponent<CharacterToken>();
            }

            return null;
        }
    }
}
