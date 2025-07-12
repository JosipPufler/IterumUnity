using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.DTOs;
using Assets.Scripts.GameLogic.models.classes;
using Assets.Scripts.GameLogic.models.creatures;
using Assets.Scripts.GameLogic.models.races;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Managers;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.models.races;
using Iterum.Scripts.Utils.Managers;
using Newtonsoft.Json;
using SFB;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class CharacterCreator : MonoBehaviour
    {
        [Header("Character")]
        public TMP_InputField characterName;
        public TMP_InputField characterBio;
        public GameObject characterImage;
        public Toggle isPlayerCharacter;

        [Header("Class")]
        public TMP_Dropdown classDropdown;
        public GameObject classDescriptionPanel;

        [Header("Race")]
        public TMP_Dropdown raceDropdown;
        public GameObject raceDescriptionPanel;

        [Header("Stats and skills")]
        public GameObject skillPrefab;
        public TMP_Text skillDescription;
        public List<StatInputBinding> statInputBindings;
        public List<SkillInputBinding> skillInputBindings;

        [Header("Controls")]
        public Button btnCreate;
        public Button btnCancle;
        public GameObject pnlCharacterList;
        public GameObject pnlCharacterCreator;

        private readonly Dictionary<Stat, TMP_InputField> statInputs = new();
        private readonly Dictionary<Skill, Toggle> skillInputs = new();

        private readonly int maxStats = 42;
        private int totalSkills = 0;
        private readonly int maxSkills = 6;

        private string selectedFilePath;

        private List<Type> raceList = new List<Type>() { typeof(Boring) };
        private List<Type> classList = new List<Type>() { typeof(Warrior) };

        public event Action OnInitialized;
        public bool IsInitialized = false;
        private string currentId = null;

        private void Start()
        {
            raceDropdown.ClearOptions();
            List<string> typeNames = raceList.Select(t => t.Name).ToList();
            raceDropdown.AddOptions(typeNames);

            classDropdown.ClearOptions();
            typeNames = classList.Select(t => t.Name).ToList();
            classDropdown.AddOptions(typeNames);

            btnCancle.onClick.AddListener(GoBack);
            btnCreate.onClick.AddListener(CreateCreature);

            raceDescriptionPanel.SetActive(false);
            classDescriptionPanel.SetActive(false);

            characterImage.GetComponent<Button>().onClick.AddListener(OpenFileDialogAndShow);
            characterImage.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Textures/default");

            IEnumerable<Stat> stats = Stat.GetAllStats();
            foreach (StatInputBinding statBinding in statInputBindings)
            {
                Stat stat = stats.Where(x => x.Name == statBinding.stat.ToString()).First();
                statInputs[stat] = statBinding.inputField;

                statBinding.inputField.text = "5";
                statBinding.inputField.onEndEdit.AddListener(x =>
                {
                    if (isPlayerCharacter.isOn)
                    {
                        int totalStats = GetTotalStats();
                        Debug.Log(totalStats);
                        if (totalStats > maxStats)
                        {
                            statBinding.inputField.text = (int.Parse(x) - (totalStats - maxStats)).ToString();
                        }
                    }
                });
            }

            IEnumerable<Skill> skills = Skill.GetAllSkills();
            foreach (SkillInputBinding skillBinding in skillInputBindings)
            {
                Stat stat = stats.Where(x => x.Name == skillBinding.skill.ToString()).First();
                foreach (var skill in skills.Where(skill => skill.Stat == stat))
                {
                    var entry = Instantiate(skillPrefab, skillBinding.panel.transform);

                    UIOnHover uIOnHover = entry.AddComponent<UIOnHover>();
                    uIOnHover.onHoverEnter += () => 
                    {
                        skillDescription.text = skill.Name;
                    };

                    uIOnHover.onHoverExit += () =>
                    {
                        skillDescription.text = "";
                    };

                    entry.transform.Find("Label").GetComponent<Text>().text = skill.Name;
                    Toggle toggle = entry.GetComponent<Toggle>();

                    skillInputs[skill] = toggle;
                    toggle.isOn = false;
                    toggle.onValueChanged.AddListener(newValue =>
                    {
                        if (totalSkills == maxSkills && newValue)
                        {
                            toggle.SetIsOnWithoutNotify(false);
                            return;
                        }
                        else if (newValue)
                        {
                            totalSkills++;
                        }
                        else
                        {
                            totalSkills--;
                        }
                    });
                }
            }
            OnInitialized?.Invoke();
            IsInitialized = true;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                GoBack();
        }

        public void GoBack() { 
            pnlCharacterCreator.SetActive(false);
            pnlCharacterList.SetActive(true);
            pnlCharacterList.GetComponent<CharacterList>().RefreshCharacters();
        }

        public void CleanUp() {
            selectedFilePath = "Textures/default";
            currentId = null;

            foreach (Toggle toggle in skillInputs.Values)
            {
                toggle.isOn = false;
            }

            foreach (TMP_InputField inputField in statInputs.Values)
            {
                inputField.text = "5";
            }

            characterName.text = "";
            characterBio.text = "";
            characterImage.GetComponent<RawImage>().texture = Resources.Load<Texture2D>(selectedFilePath);
        }

        public void CreateCreature() {
            Dictionary<Stat, int> statValues = statInputs
                .ToDictionary(
                    pair => pair.Key,
                    pair => int.TryParse(pair.Value.text, out int value) ? value : 0
                );
            Type selectedRace = raceList[raceDropdown.value];

            object instance = Activator.CreateInstance(selectedRace);
            BaseRace raceInstance = instance as BaseRace;

            Type selectedClass = classList[raceDropdown.value];

            instance = Activator.CreateInstance(selectedClass) as BaseClass;
            BaseClass classInstance = instance as BaseClass;

            AssetManager.Instance.UploadImage(selectedFilePath, null, null);
            selectedFilePath = $"{PlayerPrefs.GetString("username")}/images/{Path.GetFileName(selectedFilePath)}";
            IDownableCreature character = new(raceInstance, characterName.text, selectedFilePath, characterBio.text)
            {
                IsPlayer = isPlayerCharacter.isOn
            };
            character.SetBaseStats(statValues);
            foreach (var skill in skillInputs.Where(x => x.Value.isOn))
            {
                character.ProficiencyManager.AddSkillProficiency(skill.Key);
            }

            bool classJoined = character.ClassManager.StartClass(selectedClass);

            string serializedCharacter = JsonConvert.SerializeObject(character, JsonSerializerSettingsProvider.GetSettings());

            if (currentId == null)
            {
                CharacterManager.Instance.CreateCharacter(new CharacterDto(currentId, character.Name, 1, character.IsPlayer, serializedCharacter), (_) => GoBack(), (e) => Debug.Log(e));
            }
            else
            {
                CharacterManager.Instance.UpdateCharacter(new CharacterDto(currentId, character.Name, 1, character.IsPlayer, serializedCharacter), GoBack, (e) => Debug.Log(e));
            }
        }

        private int GetTotalStats()
        {
            int total = 0;
            foreach (var input in statInputBindings)
            {
                if (int.TryParse(input.inputField.text, out int value))
                {
                    total += value;
                }
            }
            return total;
        }

        public void OpenFileDialogAndShow()
        {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Select an Image", "", new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
            }, false);

            if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                Texture2D texture2D = LoadTextureFromFile(paths[0]);
                if (texture2D != null)
                {
                    selectedFilePath = paths[0];
                    characterImage.GetComponent<RawImage>().texture = texture2D;
                }
            }
        }

        public static Texture2D LoadTextureFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File does not exist at path: {filePath}");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new(2, 2);
            if (texture.LoadImage(fileData))
            {
                return texture;
            }

            Debug.LogError("Failed to load image data into texture.");
            return null;
        }

        public void LoadCreature(ICreature creature, string currentId) {
            this.currentId = currentId;

            characterName.text = creature.Name;
            characterBio.text = creature.Description;

            selectedFilePath = creature.ImagePath;
            AssetManager.Instance.GetImage(creature.ImagePath, (texture) => characterImage.GetComponent<RawImage>().texture = texture, null);
            IClass characterClass = creature.ClassManager.classes.First();

            classDropdown.SetValueWithoutNotify(classList.FindIndex(x => x == characterClass.GetType()));
            raceDropdown.SetValueWithoutNotify(raceList.FindIndex(x => x == creature.Race.GetType()));

            foreach (Skill skill in creature.ProficiencyManager.SkillProficiencies.Keys) {
                if (skillInputs.TryGetValue(skill, out Toggle toggle))
                {
                    toggle.isOn = true;
                }
            }

            isPlayerCharacter.isOn = creature.IsPlayer;

            foreach (var stat in statInputs)
            {
                if (creature.ModifierManager.BaseAttributes.TryGetValue(stat.Key.Attribute, out int value))
                {
                    stat.Value.text = (value + 5).ToString();
                }
            }
        }
    }

    [System.Serializable]
    public struct StatInputBinding
    {
        public StatEnum stat;
        public TMP_InputField inputField;
    }

    [System.Serializable]
    public struct SkillInputBinding
    {
        public StatEnum skill;
        public GameObject panel;
    }
}
