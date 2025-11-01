using Assets.DTOs;
using Assets.Scripts.Network;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.converters;
using Assets.Scripts.Utils.Managers;
using Iterum.models.interfaces;
using Iterum.Scripts.UI;
using Iterum.Scripts.Utils.Managers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class JoinLobby : MonoBehaviour
    {
        public GameObject characterPrefab;
        public GameObject charactersPanel;
        public ToggleGroup toggleGroup;

        public TMP_InputField ifCode;
        public Button btnJoinSession;

        private void Start()
        {
            CharacterManager.Instance.GetCharacters(AddCharacters, OnError);

            btnJoinSession.onClick.AddListener(() => {
                TryJoinSession();
            });
        }

        private void AddCharacters(List<CharacterDto> characterDtos) {
            foreach (CharacterDto characterDto in characterDtos) { 
                AddCharacter(characterDto);
            }
        }

        private void AddCharacter(CharacterDto characterDto) {
            if (ConverterUtils.TryParseCreature(characterDto.Data, out BaseCreature creature))
            {
                GameObject characterPanel = Instantiate(characterPrefab, charactersPanel.transform);
                RawImage image = characterPanel.transform.Find("Image").GetComponent<RawImage>();
                TextureMemorizer.LoadTexture(creature.ImagePath, texture => image.texture = texture);

                characterPanel.GetComponent<DataHolder>().data = characterDto;
                characterPanel.transform.Find("CharacterName").GetComponent<TMP_Text>().text = creature.Name;
                characterPanel.transform.Find("Toggle").GetComponent<Toggle>().group = toggleGroup;
            }
        }

        void TryJoinSession()
        {
            Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();
            if (string.IsNullOrEmpty(ifCode.text) || selectedToggle == null)
            {
                return;
            }
            SessionManager.Instance.JoinSession(ifCode.text, OnJoinSession, OnError);

            //StartCoroutine(WaitForLocalPlayerAndJoinSession(ifCode.text));
        }

        void OnJoinSession(SessionDto sessionDto)
        {
            Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();
            GameManager.Instance.SelectedCharacter = (CharacterDto)selectedToggle.transform.parent.GetComponent<DataHolder>().data;
            GameManager.Instance.Session = sessionDto;
            NetworkManager.singleton.networkAddress = sessionDto.ConnectionIp;
            ((TelepathyTransport)NetworkManager.singleton.transport).port = (ushort)sessionDto.ConnectionPort;
            NetworkManager.singleton.StartClient();
        }

        IEnumerator WaitForLocalPlayerAndJoinSession(string code)
        {
            while (NetworkClient.connection.identity == null)
                yield return null;

            var roomPlayer = NetworkClient.connection.identity.GetComponent<CustomRoomPlayer>();
            roomPlayer.CmdSetName(SessionData.Username);
            roomPlayer.CmdSetImagePath("Textures/mmmChip");
            roomPlayer.CmdSetCharacterName("steve");
        }

        public void OnError(string error)
        {
            Debug.LogError(error);
        }

    }
}
