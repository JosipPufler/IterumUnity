using Assets.Scripts.Utils.converters;
using Iterum.models.interfaces;
using Iterum.Scripts.UI;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class CustomRoomPlayer : NetworkRoomPlayer
    {
        [SyncVar(hook = nameof(PlayerNameChange))] public string playerName;
        [SyncVar(hook = nameof(CharacterNameChange))] public string characterName;
        [SyncVar(hook = nameof(OnImagePathChange))] public string characterImagePath;
        [SyncVar(hook = nameof(OnHostChange))] public bool host;
        [SyncVar] public string sessionCode;

        public GameObject UiPrefab;
        private GameObject uiRoomPlayerInstance;
        private RoomPlayerUI uiPrefabScript;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            CmdSetName(PlayerPrefs.GetString("username"));
            CmdSetSessionCode(GameManager.Instance.Session.SessionCode);
            if (GameManager.Instance.Session.Host == PlayerPrefs.GetString("username"))
            {
                CmdSetImagePath("Textures/GM");
                CmdSetCharacterName("Dungeon Master");
                CmdSetHost(true);
            }
            else
            {
                if (ConverterUtils.TryParseCreature(GameManager.Instance.SelectedCharacter.Data, out BaseCreature creature))
                {
                    CmdSetImagePath(creature.ImagePath);
                    CmdSetCharacterName(creature.Name);
                    CmdSetHost(false);
                }
                
            }
        }

        void PlayerNameChange(string _, string newName)
        {
            if (uiPrefabScript == null) return;
            uiPrefabScript.SetName(newName);
        }

        void CharacterNameChange(string _, string newName)
        {
            if (uiPrefabScript == null) return;
            uiPrefabScript.SetCharacterName(newName);
        }

        void OnImagePathChange(string _, string newPath)
        {
            if (uiPrefabScript == null) return;
            uiPrefabScript.SetCharacterImage(newPath);
        }

        void OnHostChange(bool _, bool newHost)
        {
            host = newHost;
            MoveUIToCorrectPanel();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (uiRoomPlayerInstance == null)
            {
                uiRoomPlayerInstance = Instantiate(UiPrefab, CanvasUI.GetPlayersPanel());
                uiPrefabScript = uiRoomPlayerInstance.GetComponent<RoomPlayerUI>();
            }

            UpdateDisplay();
            MoveUIToCorrectPanel();
        }

        private void MoveUIToCorrectPanel()
        {
            if (uiRoomPlayerInstance != null)
            {
                uiRoomPlayerInstance.transform.SetParent(host ? CanvasUI.GetHostPanel() : CanvasUI.GetPlayersPanel(), false);
            }
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            base.ReadyStateChanged(oldReadyState, newReadyState);
        }

        private void UpdateDisplay()
        {
            if (!string.IsNullOrEmpty(characterImagePath))
                uiPrefabScript.SetCharacterImage(characterImagePath);
            uiPrefabScript.SetCharacterName(characterName);
            uiPrefabScript.SetName(playerName);
        }

        [Command]
        public void CmdSetName(string name)
        {
            playerName = name;
        }

        [Command]
        public void CmdSetCharacterName(string name)
        {
            characterName = name;
        }

        [Command]
        public void CmdSetImagePath(string path)
        {
            characterImagePath = path;
        }

        [Command]
        public void CmdSetSessionCode(string code)
        {
            sessionCode = code;
        }

        [Command]
        public void CmdSetHost(bool host)
        {
            this.host = host;

            this.host = host;

            if (host && isServer)
            {
                CustomNetworkRoomManager.ServerHostUsername = playerName;
            }
        }

        [Command]
        public void CmdStartGame()
        {
            if (!isServer) return;

            string username = playerName;
            string serverKnownHost = CustomNetworkRoomManager.ServerHostUsername;

            if (username == serverKnownHost)
            {
                var manager = (CustomNetworkRoomManager)NetworkManager.singleton;
                manager.ServerChangeScene(manager.GameplayScene);
            }
            else
            {
                Debug.LogWarning($"[Server] Non-host tried to start game: {username}");
            }
        }
    }
}
