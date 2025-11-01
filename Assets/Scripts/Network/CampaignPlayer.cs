using Assets.DTOs;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.Network;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.converters;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Mirror;
using Newtonsoft.Json;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Campaign
{
    public class CampaignPlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnHostStatusChanged))]
        public bool isCampaignHost = false;

        [SyncVar] public string playerName;
        [SyncVar] public string sessionId;
        [SyncVar] public bool hasSpawnedToken = false;
        public GameObject characterTokenPrefab;
        public GameObject cameraRig;
        private UserChat _chat;

        public static CampaignPlayer LocalPlayer { get; private set; }

        public override void OnStartLocalPlayer()
        {
            LocalPlayer = this;

            CameraController cameraController = cameraRig.GetComponent<CameraController>();
            cameraController.enabled = true;
            cameraRig.GetComponentInChildren<Camera>().gameObject.SetActive(true);
            CameraController.Instance = cameraController;
            _chat = FindFirstObjectByType<UserChat>();
            StartCoroutine(DisableOtherPlayerCameras());
        }

        private IEnumerator DisableOtherPlayerCameras()
        {
            yield return new WaitForSeconds(0.2f);

            foreach (var player in FindObjectsByType<CampaignPlayer>(FindObjectsSortMode.None))
            {
                if (player != this)
                {
                    if (player.cameraRig != null)
                    {
                        var cam = player.cameraRig.GetComponentInChildren<Camera>();
                        if (cam != null)
                        {
                            cam.enabled = false;
                            cam.gameObject.SetActive(false);
                            Debug.Log($"Disabled camera for non-local player: {player.playerName}");
                        }

                        if (player.cameraRig.TryGetComponent<CameraController>(out var controller))
                            controller.enabled = false;
                    }
                }
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isLocalPlayer)
            {
                AttachHostUI();
            }
        }

        void OnHostStatusChanged(bool _, bool newValue)
        {
            if (isLocalPlayer && newValue)
            {
                AttachHostUI();
            }
        }

        private void AttachHostUI()
        {
            var ui = FindFirstObjectByType<CampaignHostPanelManager>();
            if (ui != null)
            {
                ui.AttachToLocalPlayer(this);
            }
        }

        public void TrySpawnCharacter(CharacterDto characterDto, GridCoordinate position, Team team)
        {
            if (!isLocalPlayer) return;

            CmdRequestSpawnToken(JsonConvert.SerializeObject(characterDto, JsonSerializerSettingsProvider.GetSettings()), position, true, team);
        }

        public void TrySpawnCreature(string creatureId, GridCoordinate position, Team team)
        {
            if (!isLocalPlayer || !isCampaignHost) return;
            Debug.Log(team);
            CmdRequestSpawnToken(creatureId, position, false, team);
        }

        [Command]
        void CmdRequestSpawnToken(string jsonData, GridCoordinate position, bool isCharacter, Team team)
        {
            if (!isCampaignHost && hasSpawnedToken)
            {
                Debug.LogWarning("You already spawned a token.");
                return;
            }

            if (isCharacter)
                SpawnCustomCreature(jsonData, position, team);
            else
                SpawnCreature(jsonData, position, team);

            if (!isCampaignHost)
                hasSpawnedToken = true;
        }

        [Command]
        public void CmdRequestEndTurn()
        {
            if (isServer && TurnOrderManager.Instance != null)
                TurnOrderManager.Instance.EndTurn();
        }

        [Command]
        public void CmdRequestStartCombat()
        {
            if (!isServer) return;
            Debug.Log("Combat");
            var combatants = GeneralManager.Instance.layoutManager.GetCombatants();

            TurnOrderManager.Instance.StartCombat(combatants);
        }

        [Command]
        public void CmdSendChatLinkEntry(string label, string path, string type)
        {
            RpcReceiveChatLinkEntry(label, path, type);
        }

        [Command]
        public void CmdRollSkillcheck(uint characterTokenId, Attribute skillAttribute) {
            if (!NetworkServer.spawned.TryGetValue(characterTokenId, out NetworkIdentity identity))
                return;

            if (identity.connectionToClient != connectionToClient)
            {
                Debug.LogWarning($"Client tried to roll for a character they don't own (netId {characterTokenId})");
                return;
            }

            var creature = identity.GetComponent<CharacterToken>().creature;
            Skill skill = Skill.FromAttribute(skillAttribute);
            int result = creature.Skillcheck(skill, RollType.Normal);
            RpcReceiveChatMessage($"{creature.Name} rolled a {result} for {skill.Name}");
        }

        [ClientRpc]
        void RpcReceiveChatLinkEntry(string label, string path, string type)
        {
            var chatManager = FindFirstObjectByType<CampaignHostPanelManager>();
            chatManager.AddChatLinkEntry(label, path, type);
        }

        [Command(requiresAuthority = false)]
        public void CmdSendChatMessage(string username, string message)
        {
            string fullMessage = $"{username}: {message}";
            RpcReceiveChatMessage(fullMessage);
        }

        [ClientRpc]
        void RpcReceiveChatMessage(string fullMessage)
        {
            if (_chat != null)
                _chat.AddEntry(fullMessage, true);
        }

        [Server]
        public void SpawnCreature(string creatureType, GridCoordinate position, Team team)
        {
            var player = connectionToClient.identity.GetComponent<CampaignPlayer>();

            var token = Instantiate(characterTokenPrefab);
            var ct = token.GetComponent<CharacterToken>();

            if (CreatureFactory.builtIns.ContainsKey(creatureType) &&
                CreatureFactory.TryCreate(creatureType, out BaseCreature creature))
            {
                creature.CurrentPosition = position;
                ct.creature = creature;
                ct.creature.Spawn();
                ct.UpdateCreatureJson();
                ct.controllerName = player.playerName;
                Debug.Log("[Server] Will assign creature: " + JsonConvert.SerializeObject(creature, JsonSerializerSettingsProvider.GetSettings()));
            }
            token.transform.position = CampaignGridLayout.Instance.GetPositionForTokenInRealWorld(position, token);
            ct.controllerName = player.playerName;
            ct.team = team;
            ct.position = position;
            ct.ApplyTeamStyle(ct.team);

            Debug.Log($"[CampaignManager] Spawning creature '{creatureType}' for {player.playerName}");
            NetworkServer.Spawn(token, connectionToClient);
        }

        [Command]
        public void CmdUpdateTokenHighlight(uint characterTokenId, bool outline)
        {
            if (!NetworkServer.spawned.TryGetValue(characterTokenId, out NetworkIdentity identity))
                return;

            CharacterToken characterToken = identity.GetComponent<CharacterToken>();
            characterToken.userOutline = outline;
        }

        [Server]
        public void SpawnCustomCreature(string characterJson, GridCoordinate position, Team team)
        {
            var player = connectionToClient.identity.GetComponent<CampaignPlayer>();

            var token = Instantiate(characterTokenPrefab);
            var ct = token.GetComponent<CharacterToken>();

            CharacterDto characterDto = JsonConvert.DeserializeObject<CharacterDto>(characterJson);
            if (characterDto == null)
                return;

            if (!ConverterUtils.TryParseCharacter(characterDto.Data, out DownableCreature character))
                return;
            character.InitHelpers();

            character.GetCustomActions(() =>
            {
                ct.creature = character;
                ct.creature.CurrentPosition = position;
                ct.creature.CharacterId = characterDto.Id;
                ct.UpdateCreatureJson();
                ct.controllerName = player.playerName;
                ct.team = team;
                ct.position = position;
                ct.ApplyTeamStyle(ct.team);

                token.transform.position = CampaignGridLayout.Instance.GetPositionForTokenInRealWorld(position, token);
                Debug.Log($"[CampaignManager] Spawning character for {player.playerName}");
                NetworkServer.Spawn(token, connectionToClient);
            });
        }
    }
}
