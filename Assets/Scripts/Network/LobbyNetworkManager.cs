using Assets.Scripts.Campaign;
using Assets.Scripts.Utils.converters;
using Mirror;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class LobbyNetworkManager : NetworkManager
    {
        bool gmAssigned = false;
        public static NetworkConnectionToClient HostConnection;

        public override void Awake()
        {
            base.Awake();

            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                Debug.Log("Running in headless mode => Starting Dedicated Server");

                autoCreatePlayer = false;
                StartServer();
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Debug.Log("OnServerAddPlayer: Adding player to server");
            bool willBeGM = !gmAssigned;

            if (willBeGM)
            {
                gmAssigned = true;
                HostConnection = conn;
            }

            var go = Instantiate(playerPrefab);
            var cp = go.GetComponent<CampaignPlayer>();
            cp.isCampaignHost = willBeGM;

            NetworkServer.AddPlayerForConnection(conn, go);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            CreatureSerializer.Register();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            CreatureSerializer.Register();
        }
    }
}
