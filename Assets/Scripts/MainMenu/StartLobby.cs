using Assets.DTOs;
using Assets.Scripts.Utils.Managers;
using Iterum.Scripts.UI;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class StartLobby : MonoBehaviour
    {
        [Header("Main menu")]
        public GameObject mainPanel;
        public GameObject startPanel;
        public GameObject joinPanel;
        public Button btnStartPanel;

        private void Start()
        {
            btnStartPanel.onClick.AddListener(() => {
                SendCreateSessionRequest();
                mainPanel.SetActive(false);
                startPanel.SetActive(true);
            });
        }

        private void SendCreateSessionRequest()
        {
            SessionManager.Instance.CreateSession(OnSessionCreated, OnError);
        }

        private void OnSessionCreated(SessionDto sessionDto) {
            GameManager.Instance.Session = sessionDto;
            NetworkManager.singleton.networkAddress = sessionDto.ConnectionIp;
            ((TelepathyTransport)NetworkManager.singleton.transport).port = (ushort)sessionDto.ConnectionPort;
            NetworkManager.singleton.StartClient();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && startPanel.activeSelf)
            {
                startPanel.SetActive(false);
                mainPanel.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && joinPanel.activeSelf)
            {
                joinPanel.SetActive(false);
                mainPanel.SetActive(true);
            }
        }

        public void OnError(string error) { 
            Debug.LogError(error);
        }
    }
}
