// Assets/Scripts/Utils/SessionManager.cs
using Assets.Scripts.Network;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class UnitySessionManager : NetworkBehaviour
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static UnitySessionManager Instance { get; private set; }

        private Dictionary<string, CampaignSession> sessions = new();

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public string CreateSession(out CampaignSession session)
        {
            string sessionId;
            do
            {
                sessionId = GenerateSessionCode();
            } while (sessions.ContainsKey(sessionId));

            session = new CampaignSession(sessionId);
            sessions[sessionId] = session;
            return sessionId;
        }

        public CampaignSession GetSession(string sessionId)
        {
            sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        private string GenerateSessionCode()
        {
            return new string(Enumerable.Range(0, 6).Select(_ => chars[Random.Range(0, chars.Length)]).ToArray());
        }
    }
}
