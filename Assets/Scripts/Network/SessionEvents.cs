using System;
using System.Collections.Generic;

namespace Assets.Scripts.Network
{
    public static class SessionEvents
    {
        public static Action<string> OnSessionCodeReceived;
        public static Action<List<string>> OnSessionPlayersChange;
        public static Action OnSessionJoin;
    }
}
