using Assets.DTOs;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.classes;
using Assets.Scripts.GameLogic.models.creatures;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.items;
using Assets.Scripts.GameLogic.models.races;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Managers;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.models.races;
using Iterum.Scripts.Utils.Managers;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class CharacterCreator : MonoBehaviour
    {
        private static readonly int MAX_STATS = 42;
        private static readonly int BASE_MAX_SKILLS = 6;

        [Header("Character")]
        public TMP_InputField characterName;
        public TMP_InputField characterBio;
        public GameObject characterImage;
        public Toggle isPlayerCharacter;

        [Header("Class")]
        public TMP_Dropdown classDropdown;
        public GameObject classDescriptionPanel;
        public TMP_Text classDescription;

        [Header("Race")]
        public TMP_Dropdown raceDropdown;
        public GameObject raceDescriptionPanel;
        public TMP_Text raceDescription;

        [Header("Stats and skills")]
        public GameObject skillPrefab;
        public TMP_Text skillDescription;
        public List<StatInputBinding> statInputBindings;
        public List<SkillInputBinding> skillInputBindings;
        public TMP_Text statText;
        public TMP_Text skillText;

        [Header("Controls")]
        public Button btnCreate;
        public Button btnCancle;
        public GameObject pnlCharacterList;
        public GameObject pnlCharacterCreator;

        [Header("Custom actions")]
        public GameObject actionPanel;
        public TMP_Text actionDescription;
        public GameObject actionTogglePrefab;

        [Header("Items")]
        public GameObject itemEntryPrefab;
        public GameObject itemEntryPanel;
        public TMP_Text itemDescription;

        private readonly List<Toggle> actionToggles = new();
        private readonly Dictionary<BaseItem, TMP_InputField> itemInputs = new();

        private readonly Dictionary<Stat, TMP_InputField> statInputs = new();
        private readonly Dictionary<Skill, Toggle> skillInputs = new();

        private int maxSkills = BASE_MAX_SKILLS;

        private string selectedFilePath;

        private readonly List<BaseRace> raceList = new() { new Human(), new Ork(), new WoodElf(), new SeaElf() };
        private readonly List<BaseClass> classList = new() { new Warrior(), new Pyromancer() };

        public event Action OnInitialized;
        public bool IsInitialized = false;
        private bool areItemsLoaded = false;
        private bool areActionsLoaded = false;

        private string currentId = null;

        private void Start()
        {
            raceDropdown.onValueChanged.AddListener(index =>
            {
                BaseRace race = GetCurrentRace();
                raceDescription.text = race.Description;
                foreach (var input in skillInputs)
                {
                    input.Value.interactable = true;
                }

                CheckIfSkillsValid();

                foreach (Skill skill in race.RacialSkills)
                {
                    skillInputs[skill].isOn = true;
                    skillInputs[skill].interactable = false;
                }

                CheckIfSkillsValid();

                maxSkills = BASE_MAX_SKILLS + race.SkillPointPicks;
                UpdateInfoData();
            });

            classDropdown.onValueChanged.AddListener(index =>
            {
                classDescription.text = classList.ElementAt(index).Description;
                UpdateInfoData();
            });

            raceDropdown.ClearOptions();
            List<string> typeNames = raceList.Select(x => x.Name).ToList();
            raceDropdown.AddOptions(typeNames);
            raceDropdown.onValueChanged.Invoke(raceDropdown.value);

            classDropdown.ClearOptions();
            typeNames = classList.Select(x => x.ClassName).ToList();
            classDropdown.AddOptions(typeNames);
            classDropdown.onValueChanged.Invoke(classDropdown.value);

            btnCancle.onClick.AddListener(GoBack);
            btnCreate.onClick.AddListener(CreateCreature);

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
                        if (totalStats > MAX_STATS)
                        {
                            statBinding.inputField.text = (int.Parse(x) - (totalStats - MAX_STATS)).ToString();
                        }
                        statText.text = (MAX_STATS - GetTotalStats()).ToString();
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
                        if (GetTotalSkills() > maxSkills)
                        {
                            toggle.SetIsOnWithoutNotify(false);
                        }
                        skillText.text = (maxSkills - GetTotalSkills()).ToString();
                    });
                }
            }

            UpdateInfoData();

            ActionManager.Instance.GetActions(LoadActionOptions, e => Debug.Log(e));
            ItemManager.Instance.GetItems(LoadItemOptions, e => Debug.Log(e));
        }

        private void UpdateInfoData()
        {
            statText.text = (MAX_STATS - GetTotalStats()).ToString();
            skillText.text = (maxSkills - GetTotalSkills()).ToString();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                GoBack();
        }

        private BaseClass GetCurrentClass()
        {
            return classList.ElementAt(classDropdown.value);
        }

        private BaseRace GetCurrentRace()
        {
            return raceList.ElementAt(raceDropdown.value);
        }

        private void LoadActionOptions(List<ActionDto> actionDtos) {
            foreach (var actionDto in actionDtos)
            {
                Toggle toggle = Instantiate(actionTogglePrefab, actionPanel.transform).GetComponent<Toggle>();
                toggle.isOn = false;
                toggle.transform.Find("Label").GetComponent<Text>().text = actionDto.Name;
                DataHolder dataHolder = toggle.AddComponent<DataHolder>();
                dataHolder.data = actionDto.MapToCustomAction();
                
                UIOnHover uIOnHover = toggle.AddComponent<UIOnHover>();
                uIOnHover.onHoverEnter += () =>
                {
                    actionDescription.text = actionDto.Description;
                };

                uIOnHover.onHoverExit += () =>
                {
                    actionDescription.text = "";
                };
                actionToggles.Add(toggle);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(actionPanel.transform.parent.GetComponent<RectTransform>());

            areActionsLoaded = true;
            CheckIfInitialized();
        }

        private void LoadItemOptions(List<ItemDto> itemDtos) {
            foreach (var itemDto in itemDtos)
            {
                GameObject itemEntry = Instantiate(itemEntryPrefab, itemEntryPanel.transform);
                itemEntry.transform.GetComponentInChildren<TMP_Text>().text = itemDto.Name;
                
                UIOnHover uIOnHover = itemEntry.AddComponent<UIOnHover>();
                uIOnHover.onHoverEnter += () =>
                {
                    itemDescription.text = itemDto.Description;
                };

                uIOnHover.onHoverExit += () =>
                {
                    itemDescription.text = "";
                };
                itemInputs.Add(itemDto.MapToBaseItem(), (TMP_InputField)itemEntry.transform.GetComponentInChildren(typeof(TMP_InputField)));
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(actionPanel.transform.parent.GetComponent<RectTransform>());

            areItemsLoaded = true;
            CheckIfInitialized();
        }

        public void CheckIfInitialized() {
            if (areActionsLoaded && areItemsLoaded) { 
                IsInitialized = true;
                OnInitialized?.Invoke();
                UpdateInfoData();
            }
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

            foreach (TMP_InputField inputField in itemInputs.Values)
            {
                inputField.text = "0";
            }

            foreach (var toggle in actionToggles)
            {
                toggle.isOn = false;
            }

            characterName.text = "";
            characterBio.text = "";
            raceDropdown.value = 0;
            classDropdown.value = 0;

            characterImage.GetComponent<RawImage>().texture = Resources.Load<Texture2D>(selectedFilePath);
        }

        public void CreateCreature() {
            Dictionary<Stat, int> statValues = statInputs
                .ToDictionary(
                    pair => pair.Key,
                    pair => int.TryParse(pair.Value.text, out int value) ? value : 0
                );

            BaseRace raceInstance = GetCurrentRace();

            BaseClass classInstance = GetCurrentClass();

            AssetManager.Instance.UploadImage(selectedFilePath, null, null);
            selectedFilePath = $"{SessionData.Username}/images/{Path.GetFileName(selectedFilePath)}";
            BaseCreature creature;
            if (isPlayerCharacter.isOn)
            {
                creature = new DownableCreature(raceInstance, characterName.text, selectedFilePath, characterBio.text);
            } else {
                creature = new BaseCreature(raceInstance, characterName.text, selectedFilePath, characterBio.text);
            }

            creature.IsPlayer = isPlayerCharacter.isOn;
            creature.CustomActionIds.Clear();
            creature.CustomActions.Clear();
            foreach (var toggle in actionToggles.Where(x => x.isOn))
            {
                CustomBaseAction action = (CustomBaseAction)toggle.GetComponent<DataHolder>().data;
                creature.CustomActionIds.Add(action.Id);
                creature.CustomActions.Add(action);
            }
            
            creature.SetBaseStats(statValues);
            foreach (var skill in skillInputs.Where(x => x.Value.isOn))
            {
                creature.ProficiencyManager.AddSkillProficiency(skill.Key);
            }

            bool classJoined = creature.ClassManager.StartClass(classInstance.GetType());

            creature.Inventory.Clear();

            foreach (var itemEntry in itemInputs)
            {
                if (int.TryParse(itemEntry.Value.text, out int numberOfItems))
                {
                    if (itemEntry.Key is BaseWeapon weapon)
                    {
                        for (int i = 0; i < numberOfItems; i++)
                        {
                            var copy = new BaseWeapon(weapon);
                            creature.Inventory.Add(copy);
                        }
                    }
                    else if (itemEntry.Key is BaseConsumable consumable)
                    {
                        for (int i = 0; i < numberOfItems; i++)
                        {
                            var copy = new BaseConsumable(consumable);
                            creature.Inventory.Add(copy);
                        }
                    }
                }
            }

            creature.CharacterId = currentId;
            creature.Spawn();

            if (currentId == null)
            {
                CharacterManager.Instance.CreateCharacter(new CharacterDto(creature), _ => GoBack(), e => Debug.Log(e));
            }
            else
            {
                CharacterManager.Instance.UpdateCharacter(new CharacterDto(creature), GoBack, e => Debug.Log(e));
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

        private int GetTotalSkills()
        {
            int total = 0;
            foreach (KeyValuePair<Skill, Toggle> input in skillInputs)
            {
                if (input.Value.isOn)
                {
                    total++;
                }
            }
            return total - GetCurrentRace().RacialSkills.Count;
        }

        private void CheckIfSkillsValid() 
        {
            if (GetTotalSkills() > maxSkills)
            {
                int difference = maxSkills - GetTotalSkills();
                int turnedOff = 0;
                foreach (var skillToggle in skillInputs.Where(x => x.Value.enabled && x.Value.isOn))
                {
                    skillToggle.Value.isOn = false;
                    turnedOff++;
                    if (turnedOff == difference)
                        return;
                }
            }
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

        public void LoadCreature(BaseCreature creature, string currentId) {
            this.currentId = currentId;

            characterName.text = creature.Name;
            characterBio.text = creature.Description;

            selectedFilePath = creature.ImagePath;
            TextureMemorizer.LoadTexture(creature.ImagePath, (texture) => characterImage.GetComponent<RawImage>().texture = texture);
            IClass characterClass = creature.ClassManager.Classes.First();

            classDropdown.value = classList.FindIndex(x => x.GetType() == characterClass.GetType());
            raceDropdown.value = raceList.FindIndex(x => x.GetType() == creature.Race.GetType());

            foreach (Skill skill in creature.ProficiencyManager.SkillProficiencies.Keys) {
                if (skillInputs.TryGetValue(skill, out Toggle toggle))
                {
                    toggle.isOn = true;
                }
            }

            foreach (var stat in statInputs)
            {
                if (creature.ModifierManager.BaseAttributes.TryGetValue(stat.Key.Attribute, out int value))
                {
                    stat.Value.text = (value + 5).ToString();
                }
            }

            foreach (var toggle in actionToggles)
            {
                CustomBaseAction action = (CustomBaseAction)toggle.GetComponent<DataHolder>().data;
                if (creature.CustomActionIds.Any(id => action.Id == id))
                {
                    toggle.isOn = true;
                }
            }
            
            isPlayerCharacter.isOn = creature.IsPlayer;

            foreach (var item in itemInputs)
            {
                int count = creature.Inventory.Count(x => x.Name == item.Key.Name);
                if (count > 0)
                {
                    item.Value.text = count.ToString(); 
                }
            }
        }
    }

    [Serializable]
    public struct StatInputBinding
    {
        public StatEnum stat;
        public TMP_InputField inputField;
    }

    [Serializable]
    public struct SkillInputBinding
    {
        public StatEnum skill;
        public GameObject panel;
    }
}
