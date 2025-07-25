using Iterum.models.interfaces;
using Mirror;

namespace Assets.Scripts.Network
{
    public class CreatureMessage : NetworkMessage
    {
        public BaseCreature creature;
    }
}
