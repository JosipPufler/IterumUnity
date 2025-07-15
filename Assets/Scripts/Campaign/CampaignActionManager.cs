using Assets.Scripts.GameLogic.models;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.target;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using kcp2k;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Campaign
{
    public class CampaignActionManager : NetworkBehaviour 
    {
        public BaseAction SelectedAction { get; set; }
        public GameObject targetPrefab;
        public GameObject content;
        public UserChat userChat;

        public bool LookingForTargets { get; set; }
        public TargetData CurrentTargetData { get; set; }
        public CharacterToken CurrentToken { get; set; }
        private Queue<TargetData> TargetDataQueue { get; set; } = new Queue<TargetData>();
        private ActionInfo ActionInfo { get; set; }

        private Func<ActionInfo, ActionResult> callback;

        public static CampaignActionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClearData();
            }
        }

        public void SetAction(IAction action, CharacterToken characterToken, Func<ActionInfo, ActionResult> callback = null)
        {
            if (action == null)
                return;

            if (!action.CanTakeAction(characterToken.creature))
            {
                return;
            }

            ClearData();

            if (callback != null)
            {
                this.callback = callback;
            }
            else
            {
                this.callback = DefaultCallback;
            }

            if (action is BaseAction baseAction)
            {
                SelectedAction = baseAction;
            }
            else
            {
                return;
            }

            CurrentToken = characterToken;
            ActionInfo = new()
            {
                OriginCreature = characterToken.creature,
            };
            LookingForTargets = true;

            TargetDataQueue.Clear();

            foreach (var targetData in SelectedAction.TargetTypes)
            {
                GameObject targetEntry = Instantiate(targetPrefab, content.transform);
                targetEntry.GetComponent<TMP_Text>().text = $"{targetData.Key.TargetType} {targetData.Key.MinDistance}-{targetData.Key.MaxDistance}: {targetData.Value}";

                for (int i = 0; i < targetData.Value; i++)
                    TargetDataQueue.Enqueue(targetData.Key);
            }
            CurrentTargetData = TargetDataQueue.Dequeue();
        }

        private void ClearData()
        {
            foreach (Transform item in content.transform)
            {
                Destroy(item.gameObject);
            }
            CurrentTargetData = null;
            TargetDataQueue.Clear();
            ActionInfo = null;
            LookingForTargets = false;
            SelectedAction = null;
            callback = null;
        }

        public void SubmitTarget(TargetDataSubmission submission)
        {
            CmdSubmitTarget(submission.Serialize()); // Send a simplified serializable version
        }

        [Command]
        private void CmdSubmitTarget(TargetDataSubmissionDTO dto)
        {
            TargetDataSubmission targetData = dto.Deserialize(); // reconstruct
            if (targetData.TargetData.ID != CurrentTargetData.ID) return;

            if (!ActionInfo.Targets.ContainsKey(CurrentTargetData))
                ActionInfo.Targets[CurrentTargetData] = new();

            ActionInfo.Targets[CurrentTargetData].Add(targetData);

            if (TargetDataQueue.TryDequeue(out var nextTarget))
            {
                CurrentTargetData = nextTarget;
            }
            else
            {
                ActionResult result = DefaultCallback(ActionInfo);
                RpcPrintActionResult(result);
                ClearData();
            }
        }


        private ActionResult DefaultCallback(ActionInfo actionInfo) {
            if (SelectedAction is CustomBaseAction customAction)
            {
                return customAction.ExecuteAction(actionInfo);
            }
            return SelectedAction.ExecuteAction(actionInfo);
        }

        [ClientRpc]
        private void RpcPrintActionResult(ActionResult result)
        {
            result.ActionMessages.ForEach(msg => userChat.AddEntry(msg, false));
        }
    }
}
