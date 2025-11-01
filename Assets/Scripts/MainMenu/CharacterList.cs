// Assets/Scripts/Utils/CharacterList.cs
using Assets.DTOs;
using Assets.Scripts.Utils;
using Iterum.models.interfaces;
using Iterum.Scripts.Utils.Managers;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class CharacterList : MonoBehaviour
    {
        public GameObject content;
        public GameObject entryPrefab;
        public CharacterCreator creator;
        public Button newCharacter;
        public GameObject mainPanel;

        private BaseCreature selectedCreature;
        private string selectedId;

        readonly IList<GameObject> entries = new List<GameObject>();

        private void Start()
        {
            newCharacter.onClick.AddListener(() => creator.CleanUp());

            RefreshCharacters();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                mainPanel.SetActive(true);
            }
        }

        public void RefreshCharacters() {
            CharacterManager.Instance.GetCharacters(ReloadCharacters, OnError);
        }

        void ReloadCharacters(IEnumerable<CharacterDto> characters)
        {
            foreach (var item in entries)
            {
                Destroy(item);
            }

            foreach (CharacterDto character in characters)
            {
                AddEntry(character);
            }
        }

        void AddEntry(CharacterDto character)
        {
            var entry = Instantiate(entryPrefab, content.transform);

            entry.transform.Find("lblName").GetComponent<TMP_Text>().text = character.Name;
            entry.transform.Find("lblLevel").GetComponent<TMP_Text>().text = character.Level.ToString();
            entry.transform.Find("lblIsPlayer").GetComponent<TMP_Text>().text = character.IsPlayer ? "Yes" : "No";
            var buttonHolder = entry.transform.Find("buttons");
            buttonHolder.Find("btnEdit").GetComponent<Button>().onClick.AddListener(() => LoadCharacter(character));
            buttonHolder.Find("btnDelete").GetComponent<Button>().onClick.AddListener(() => DeleteCharacter(character, entry.transform));

            entry.SetActive(true);

            entries.Add(entry);
        }

        void LoadCharacter(CharacterDto character)
        {
            mainPanel.SetActive(false);
            creator.gameObject.SetActive(true);
            selectedCreature = JsonConvert.DeserializeObject<BaseCreature>(character.Data, JsonSerializerSettingsProvider.GetSettings());
            selectedId = character.Id;
            if (creator.IsInitialized)
            {
                creator.LoadCreature(selectedCreature, selectedId);
            } else
                creator.OnInitialized += OnInitializedLoadCharacter;
        }

        void OnInitializedLoadCharacter() {
            creator.LoadCreature(selectedCreature, selectedId);
            creator.OnInitialized -= OnInitializedLoadCharacter;
            gameObject.SetActive(false);
        }

        void DeleteCharacter(CharacterDto characterDto, Transform transform)
        {
            CharacterManager.Instance.DeleteCharacter(characterDto.Id, () => {
                CharacterManager.Instance.GetCharacters(ReloadCharacters, OnError);
                Destroy(transform.gameObject);
            }, OnError);
        }

        void OnError(string error)
        {
            Debug.Log(error);
        }
    }
}
