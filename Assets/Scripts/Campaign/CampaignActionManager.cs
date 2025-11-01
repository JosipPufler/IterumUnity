using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.items;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.Network;
using Assets.Scripts.Utils;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Mirror;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Campaign
{
    public class CampaignActionManager : NetworkBehaviour 
    {
        public IAction SelectedAction { get; set; }
        public GameObject targetPrefab;
        public GameObject content;
        public UserChat userChat;

        [SyncVar]
        public bool LookingForTargets;
        public TargetData CurrentTargetData;
        public CharacterToken CurrentToken { get => TurnOrderManager.Instance.GetCurrentCharacter(); } 
        private Queue<TargetData> TargetDataQueue { get; set; } = new Queue<TargetData>();
        private ActionInfo ActionInfo { get; set; }

        private Func<ActionInfo, ActionResult> callback;
        private BaseConsumable Consumable { get; set; }
        public static CampaignActionManager Instance { get; private set; }

        IList<CharacterToken> HighlightedTokens { get; set; } = new List<CharacterToken>();

        private void Start()
        {
            content = GameObject.FindWithTag("TargetUIContent");
            userChat = FindAnyObjectByType<UserChat>();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            
            Instance = this;
            enabled = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CmdClearData();
            }
        }

        private ActionResult DefaultCallback(ActionInfo actionInfo)
        {
            if (SelectedAction is CustomBaseAction customAction)
            {
                return customAction.ExecuteAction(actionInfo);
            }
            return SelectedAction.ExecuteAction(actionInfo);
        }

        private ActionResult ConsumableCallback(ActionInfo actionInfo)
        {
            return Consumable.Consume(actionInfo.OriginCreature.Inventory, actionInfo);
        }

        [Command]
        public void CmdSetAction(string actionGuid) {
            CharacterToken token = CurrentToken;
            if (token == null)
            {
                return;
            }
            BaseCreature creature = token.creature;
            Debug.Log(token.creature.Name);
            IAction action = creature.GetActions().FirstOrDefault(x => x.ID == actionGuid);
            if (action != null)
            {
                SelectedAction = action;
                callback = DefaultCallback;
            }
            else
            {
                Consumable = creature.GetConsumables().FirstOrDefault(x => x.ConsumeAction.ID == actionGuid);
                if (Consumable == null)
                {
                    return;
                }
                SelectedAction = Consumable.ConsumeAction;
                callback = ConsumableCallback;
            }

            ActionInfo = new(){
                OriginCreature = creature,
            };
            LookingForTargets = true;

            TargetDataQueue.Clear();

            foreach (var targetData in SelectedAction.TargetTypes)
            {
                for (int i = 0; i < targetData.Value; i++)
                    TargetDataQueue.Enqueue(targetData.Key);
            }
            if (TargetDataQueue.TryDequeue(out CurrentTargetData))
            {
                SetTargetData(CurrentTargetData);
                string currentTargetJson = JsonConvert.SerializeObject(CurrentTargetData, JsonSerializerSettingsProvider.GetSettings());
                TargetRpcSetCurrentTarget(connectionToClient, currentTargetJson);

                string targetJson = JsonConvert.SerializeObject(SelectedAction.TargetTypes, JsonSerializerSettingsProvider.GetSettings());
                TargetRpcSetTargetData(connectionToClient, targetJson);
            }
            else
            {
                ExecuteCurrentAction();
            }
        }

        [Command]
        public void CmdEquipItem(string itemGuid)
        {
            CharacterToken token = CurrentToken;
            BaseCreature creature = token.creature;
            IItem item = creature.Inventory.FindLast(x => x.ID == itemGuid);
            if (item != null)
            {
                if (item is BaseArmor armor)
                {
                    creature.ArmorSet.DonArmor(armor, creature.Inventory);
                }
                else if (item is BaseWeapon weapon)
                {
                    creature.WeaponSet.EquipWeapon(weapon, creature.Inventory);
                }
                token.UpdateCreatureJson();
            }
        }

        [Command]
        public void CmdUnequipItem(string itemGuid)
        {
            BaseCreature creature = CurrentToken.creature;
            BaseWeapon baseWeapon = creature.WeaponSet.Weapons.First(x => x.ID == itemGuid);
            if (baseWeapon != null) {
                creature.WeaponSet.UnequipWeapon(creature.Inventory, baseWeapon);
                CurrentToken.UpdateCreatureJson();
                return;
            }

            BaseArmor baseArmor = creature.ArmorSet.GetArmor().First(x => x.ID == itemGuid);
            if (baseArmor != null)
            {
                creature.ArmorSet.DoffArmor(creature.Inventory, baseArmor);
                CurrentToken.UpdateCreatureJson();
                return;
            }
        }

        [TargetRpc]
        public void TargetRpcSetCurrentTarget(NetworkConnection conn, string currentTargetJson)
        {
            CurrentTargetData = JsonConvert.DeserializeObject<TargetData>(currentTargetJson, JsonSerializerSettingsProvider.GetSettings());
        }

        [TargetRpc]
        public void TargetRpcSetTargetData(NetworkConnection conn, string targetDataJson) {
            Dictionary<TargetData, int> targetDataDict = JsonConvert.DeserializeObject<Dictionary<TargetData, int>>(targetDataJson, JsonSerializerSettingsProvider.GetSettings());

            foreach (Transform targetData in content.transform)
            {
                Destroy(targetData.gameObject);
            }

            foreach (var targetData in targetDataDict)
            {
                GameObject targetEntry = Instantiate(targetPrefab, content.transform);
                targetEntry.GetComponent<TMP_Text>().text = $"{targetData.Key.TargetType} {targetData.Key.MinDistance}-{targetData.Key.MaxDistance}: {targetData.Value}";
            }
        }

        [Command]
        private void CmdClearData() {
            ClearData();
        }

        [Server]
        private void ClearData()
        {
            CurrentTargetData = null;
            TargetDataQueue.Clear();
            ActionInfo = null;
            LookingForTargets = false;
            SelectedAction = null;
            callback = null;
            Consumable = null;
            ClearTargetHighlights();
        }

        public void SubmitCreatureTarget(TargetDataSubmissionCreature submission)
        {
            CmdSubmitCreatureTarget(JsonConvert.SerializeObject(submission, JsonSerializerSettingsProvider.GetSettings()));
        }

        public void SubmitHexTarget(TargetDataSubmissionHex submission)
        {
            CmdSubmitHexTarget(JsonConvert.SerializeObject(submission, JsonSerializerSettingsProvider.GetSettings()));
        }

        [Command]
        private void CmdSubmitCreatureTarget(string targetDataJson) 
        {
            TargetDataSubmissionCreature targetData = JsonConvert.DeserializeObject<TargetDataSubmissionCreature>(targetDataJson, JsonSerializerSettingsProvider.GetSettings());

            ProcessTarget(targetData);
        }

        [Command]
        private void CmdSubmitHexTarget(string targetDataJson)
        {
            TargetDataSubmissionHex targetData = JsonConvert.DeserializeObject<TargetDataSubmissionHex>(targetDataJson, JsonSerializerSettingsProvider.GetSettings());
            ProcessTarget(targetData);
        }

        [Server]
        private void ProcessTarget(TargetDataSubmission targetData)
        {
            if (targetData.TargetData.ID != CurrentTargetData.ID) return;

            if (!ActionInfo.Targets.ContainsKey(CurrentTargetData))
                ActionInfo.Targets[CurrentTargetData] = new();

            ActionInfo.Targets[CurrentTargetData].Add(targetData);

            if (TargetDataQueue.TryDequeue(out var nextTarget))
            {
                ClearTargetHighlights();
                CurrentTargetData = nextTarget;
                SetTargetData(CurrentTargetData);
                string currentTargetJson = JsonConvert.SerializeObject(CurrentTargetData, JsonSerializerSettingsProvider.GetSettings());
                TargetRpcSetCurrentTarget(connectionToClient, currentTargetJson);
            }
            else
            {
                ExecuteCurrentAction();
            }
        }

        [Server]
        private void ExecuteCurrentAction()
        {
            ActionResult result = callback(ActionInfo);
            string json = JsonConvert.SerializeObject(result.ActionMessages, JsonSerializerSettingsProvider.GetSettings());
            foreach (CharacterToken token in CampaignGridLayout.Instance.GetCombatants())
            {
                token.UpdateCreatureJson();
            }

            RpcPrintActionResult(json);
            ClearData();
        }

        [Server]
        private void SetTargetData(TargetData targetData) {
            if (targetData.TargetType == TargetType.Creature)
            {
                HighlightedTokens = CampaignGridLayout.Instance.GetTokensInRing(CurrentToken.position, targetData.MinDistance, targetData.MaxDistance).ToList();
                foreach (var creatureToken in HighlightedTokens)
                {
                    creatureToken.forceOutline = true;
                }
            }
        }

        [Server]
        private void ClearTargetHighlights() {
            foreach (var creatureToken in HighlightedTokens)
            {
                creatureToken.forceOutline = false;
            }
            HighlightedTokens.Clear();
        }

        [ClientRpc]
        private void RpcPrintActionResult(string resultJson)
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            var result = JsonConvert.DeserializeObject<List<string>>(resultJson, JsonSerializerSettingsProvider.GetSettings());
            result.ForEach(msg => userChat.AddEntry(msg, false));
        }
    }
}
