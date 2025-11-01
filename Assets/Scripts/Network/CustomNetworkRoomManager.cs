using Assets.Scripts.Campaign;
using Assets.Scripts.Utils.converters;
using Iterum.DTOs;
using Iterum.Scripts.UI;
using Iterum.Scripts.Utils;
using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting;
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
            string[] args = Environment.GetCommandLineArgs();
            string adminUsername = null;
            string adminPassword = null;

            if (GameManager.Instance == null)
            {
                this.AddComponent<GameManager>();
            }

            _telepathy = transport as TelepathyTransport;
            _telepathy.clientMaxMessageSize = 65535;
            _telepathy.serverMaxMessageSize = 65535;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-port" && i + 1 < args.Length)
                {
                    if (ushort.TryParse(args[i + 1], out var p))
                    {
                        _telepathy.port = p;
                    }
                }

                if (args[i] == "-adminUsername" && i + 1 < args.Length)
                {
                    adminUsername = args[i + 1];
                }

                if (args[i] == "-adminPassword" && i + 1 < args.Length)
                {
                    adminPassword = args[i + 1];
                }
            }

            if (adminUsername != null && adminPassword != null) {
                UserManager.Instance.Login(new LoginForm(adminUsername, adminPassword), (loginResponse) => Debug.Log(loginResponse.JwtToken), (e) => Debug.Log(e));
            }
            else
            {
                Debug.Log("Something wrong with admin credentials");
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

            yield return null;

            foreach (var roomPlayer in roomSlots)
            {
                var customRoomPlayer = roomPlayer as CustomRoomPlayer;
                var conn = customRoomPlayer.connectionToClient;

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

                if (conn.identity.TryGetComponent<CampaignPlayer>(out var campaignPlayer))
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
