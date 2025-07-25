using Assets.Scripts.Campaign;
using Assets.Scripts.Utils.converters;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class CustomNetworkRoomManager : NetworkRoomManager
    {
        private static CustomNetworkRoomManager instance;

        TelepathyTransport _telepathy;
        public static string ServerHostUsername;
        public GameObject tokenPrefab;

        public override void Awake()
        {
            var args = Environment.GetCommandLineArgs();

            _telepathy = transport as TelepathyTransport;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-port" && i + 1 < args.Length)
                {
                    if (ushort.TryParse(args[i + 1], out var p))
                    {
                        _telepathy.port = p;
                    }
                }
            }

            if (instance != null && instance != this)
            {
                Debug.LogWarning("Duplicate NetworkManager found, destroying new one.");
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnRoomServerAddPlayer(conn);

            var player = conn.identity.GetComponent<CustomRoomPlayer>();
            if (player.host)
            {
                ServerHostUsername = player.playerName;
                Debug.Log("Host registered on server: " + ServerHostUsername);
            }
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            base.OnRoomServerSceneChanged(sceneName);

            if (sceneName == GameplayScene)
            {
                StartCoroutine(InitializeGameplayPlayers());
            }
        }

        private IEnumerator InitializeGameplayPlayers()
        {
            Debug.Log("[Server] Waiting for CampaignPlayers to initialize...");

            yield return null; // Wait one frame to allow scene load

            foreach (var roomPlayer in roomSlots)
            {
                var customRoomPlayer = roomPlayer as CustomRoomPlayer;
                var conn = customRoomPlayer.connectionToClient;

                // Wait for the new identity to be assigned
                float timeout = 3f;
                float timer = 0f;
                while ((conn.identity == null || conn.identity.GetComponent<CampaignPlayer>() == null) && timer < timeout)
                {
                    yield return null;
                    timer += Time.deltaTime;
                }

                if (conn.identity == null)
                {
                    Debug.LogWarning($"[Server] Identity still null after timeout for conn {conn.connectionId}");
                    continue;
                }

                var campaignPlayer = conn.identity.GetComponent<CampaignPlayer>();
                if (campaignPlayer != null)
                {
                    campaignPlayer.playerName = customRoomPlayer.playerName;
                    campaignPlayer.sessionId = customRoomPlayer.sessionCode;
                    campaignPlayer.isCampaignHost = customRoomPlayer.host;
                    
                    Debug.Log($"[Server] Assigned data to CampaignPlayer (conn {conn.connectionId}): " +
                              $"Name={campaignPlayer.playerName}, Session={campaignPlayer.sessionId}, Host={campaignPlayer.isCampaignHost}");
                }
                else
                {
                    Debug.LogWarning($"[Server] CampaignPlayer not found on identity for conn {conn.connectionId}");
                }
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log($"Server listening on port {_telepathy.port}");
            CreatureSerializer.Register();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            CreatureSerializer.Register();
        }

        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerConnect(conn);
            Debug.Log($"Player connected: {conn.connectionId}");
        }
    }
}
