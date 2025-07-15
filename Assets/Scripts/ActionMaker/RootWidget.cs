using Assets.DTOs;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Assets.Scripts.Utils;
using Iterum.Scripts.UI;
using Iterum.Scripts.Utils.Managers;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.ActionMaker
{
    public class RootWidget : MonoBehaviour
    {
        [Header("Fields")]
        public TMP_InputField ifName;
        public TMP_InputField ifDescription;
        public TMP_InputField ifAp;
        public TMP_InputField ifMp;
        public GameObject content;

        [Header("Prefabs")]
        public GameObject commandPrefab;
        public GameObject attackPrefab;
        public GameObject savingThrowData;

        private string currentId;

        private void Start()
        {
            if (GameManager.Instance.SelectedAction != null) {
                LoadAction(GameManager.Instance.SelectedAction);
            }
            else
            {
                currentId = null;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBack();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                SaveAction();
            }
        }

        private static void GoBack()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public CustomBaseAction GetAction() { 
            return new CustomBaseAction() { 
                Name = ifName.text,
                Description = ifDescription.text,
                ApCost = int.Parse(ifAp.text),
                MpCost = int.Parse(ifMp.text),
                CustomTargetTypes = GetActionData()
            };
        }

        public Dictionary<CustomTargetData, ActionPackage> GetActionData() {
            Dictionary<CustomTargetData, ActionPackage> targetData = new();
            foreach (Transform child in content.transform)
            {
                if (child.TryGetComponent<MakerWidget>(out var widget))
                {
                    targetData.Add(widget.GetCustomTargetData(), widget.GetActionPackage());
                }
            }
            return targetData;
        }

        public void SaveAction() {
            CustomBaseAction newAction = GetAction();
            string serializedAction = JsonConvert.SerializeObject(newAction, JsonSerializerSettingsProvider.GetSettings());

            if (currentId == null)
            {
                ActionManager.Instance.CreateAction(new ActionDto(newAction), (action) => OnCreate(action), (e) => Debug.Log(e));
            }
            else
            {
                ActionManager.Instance.UpdateAction(new ActionDto(newAction, currentId), null, (e) => Debug.Log(e));
            }
        }

        private void OnCreate(ActionDto action) { 
            currentId = action.Id;
        }

        public void LoadAction(ActionDto action) {
            currentId = action.Id;
            ifName.text = action.Name;
            ifDescription.text = action.Description;
            ifMp.text = action.MpCost.ToString();
            ifAp.text = action.ApCost.ToString();

            CustomBaseAction customBaseAction = JsonConvert.DeserializeObject<CustomBaseAction>(action.Data, JsonSerializerSettingsProvider.GetSettings());
            foreach (var kvp in customBaseAction.CustomTargetTypes)
            {
                if (kvp.Key.ActionType == ActionType.Command)
                {
                    MakerWidget commandWidget = Instantiate(commandPrefab, content.transform).GetComponent<MakerWidget>();
                    commandWidget.LoadAction(kvp);
                }
                else if (kvp.Key.ActionType == ActionType.Attack)
                {
                    AttackWidget attackWidget = Instantiate(attackPrefab, content.transform).GetComponent<AttackWidget>();
                    attackWidget.LoadAction(kvp);
                }
                else if (kvp.Key.ActionType == ActionType.SavingThrow)
                {
                    SavingThrowWidget savingThrowWidget = Instantiate(savingThrowData, content.transform).GetComponent<SavingThrowWidget>();
                    savingThrowWidget.LoadAction(kvp);
                }
            }
        }
    }
}
