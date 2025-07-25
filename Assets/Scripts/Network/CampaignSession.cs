// Assets/Scripts/Utils/CampaignSession.cs
using Assets.Scripts.Campaign;
using Iterum.DTOs;
using Mirror;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Network
{
    public enum SessionPhase
    {
        Lobby,
        InGame,
        Ended
    }

    public class CampaignSession
    {
        public string SessionId { get; private set; }

        public Dictionary<NetworkConnectionToClient, CampaignPlayer> Players = new();

        public MapDto CurrentMap;

        public NetworkCampaignGrid Grid;

        public NetworkConnectionToClient HostConnection;

        // Any session-specific settings (fog of war, turn timers, etc.)
        //public CampaignSettings Settings;

        public SessionPhase Phase;

        public CampaignSession(string sessionId)
        {
            SessionId = sessionId;
            Phase = SessionPhase.Lobby;
        }

        public void AddPlayer(NetworkConnectionToClient conn, CampaignPlayer player)
        {
            if (!Players.ContainsKey(conn))
            {
                Players[conn] = player;
                HostConnection ??= conn;
                NotifyPlayersChange();
            }
        }

        private void NotifyPlayersChange()
        {
            List<string> playerNames = Players.Select(p => p.Value.playerName).ToList();
            SessionEvents.OnSessionPlayersChange?.Invoke(playerNames);
        }

        public void RemovePlayer(NetworkConnectionToClient conn)
        {
            if (Players.Remove(conn))
            {
                NotifyPlayersChange();
                if (conn == HostConnection)
                    HostConnection = Players.Keys.FirstOrDefault();
            }
        }
    }
}
