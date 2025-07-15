using Assets.DTOs;
using Iterum.Scripts.UI;
using Iterum.Scripts.Utils.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class ActionList : MonoBehaviour
    {
        public GameObject content;
        public GameObject entryPrefab;
        public Button newAction;
        public GameObject pnlMain;

        private void Start()
        {
            newAction.onClick.AddListener(() => SceneManager.LoadScene("ActionMaker"));

            RefreshActions();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { 
                pnlMain.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        public void RefreshActions()
        {
            ActionManager.Instance.GetActions(ReloadActions, OnError);
        }

        void ReloadActions(IEnumerable<ActionDto> actions)
        {
            foreach (GameObject item in content.transform)
            {
                Destroy(item);
            }

            foreach (ActionDto action in actions)
            {
                AddEntry(action);
            }
        }

        void AddEntry(ActionDto action)
        {
            var entry = Instantiate(entryPrefab, content.transform);

            entry.transform.Find("lblName").GetComponent<TMP_Text>().text = action.Name;
            entry.transform.Find("lblDescription").GetComponent<TMP_Text>().text = action.Description;
            
            entry.transform.Find("btnEdit").GetComponent<Button>().onClick.AddListener(() => {
                GameManager.Instance.SelectedAction = action;
                SceneManager.LoadScene("ActionMaker");
            });
            entry.transform.Find("btnDelete").GetComponent<Button>().onClick.AddListener(() => DeleteAction(action, entry.transform));

            entry.SetActive(true);
        }

        void DeleteAction(ActionDto actionDto, Transform transform)
        {
            ActionManager.Instance.DeleteAction(actionDto.Id, () => {
                ActionManager.Instance.GetActions(ReloadActions, OnError);
                Destroy(transform.gameObject);
            }, OnError);
        }

        void OnError(string error)
        {
            Debug.Log(error);
        }
    }
}
