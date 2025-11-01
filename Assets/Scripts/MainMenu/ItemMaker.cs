using Assets.DTOs;
using Assets.Scripts.ActionMaker;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.items;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Managers;
using Iterum.models;
using Iterum.models.actions;
using Iterum.models.enums;
using Iterum.Scripts.Utils.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public enum ItemType { 
        Consumable,
        Weapon
    }

    public class ItemMaker : MonoBehaviour
    {   
        public Button btnCreate;
        public Button btnCancle;
        public TMP_InputField ifDescription;
        public GameObject parentPanel;
        public GameObject pnlWeapon;

        [Header("Item traits")]
        public TMP_Dropdown itemTypeDropdown;
        public TMP_InputField IfWeight;
        public TMP_InputField ifName;

        [Header("Action")]
        public GameObject ActionEntryPrefab;
        public GameObject actionPanel;
        public TMP_Text actionDescription;
        private readonly Dictionary<Toggle, BaseAction> actionToggles = new();
        
        [Header("Weapon traits")]
        public GameObject weaponTraitEntryPrefab;
        public GameObject weaponTraitParentPanel;
        public TMP_Text weaponTraitDescription;
        public Dictionary<WeaponTrait, Toggle> weaponTraits = new();
        public StaticDamageEntry weaponDamageEntry;
        public TMP_Dropdown weaponTypesDropdown;

        [Header("Error")]
        public MainMenuNotifPanel errorPanel;

        public event Action OnInitialized;
        public bool IsInitialized = false;
        private string currentId = null;

        private KeyValuePair<Toggle, BaseAction> lastAction;

        private void Start()
        {
            btnCreate.onClick.AddListener(CreateItem);
            btnCancle.onClick.AddListener(GoBack);

            itemTypeDropdown.ClearOptions();
            itemTypeDropdown.AddOptions(Enum.GetValues(typeof(ItemType)).Cast<ItemType>().Select(x => x.ToString()).ToList());
            itemTypeDropdown.onValueChanged.AddListener((newValue) => {
                if (GetCurrentItemType() == ItemType.Weapon)
                {
                    pnlWeapon.SetActive(true);
                } else
                {
                    pnlWeapon.SetActive(false);
                }

                if (GetCurrentItemType() == ItemType.Consumable)
                {
                    foreach (var toggle in actionToggles)
                    {
                        toggle.Key.isOn = false;
                    }
                    if (lastAction.Key != null)
                    {
                        lastAction.Key.isOn = true;
                    }
                }
            });

            weaponTypesDropdown.ClearOptions();
            weaponTypesDropdown.AddOptions(Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().Select(x => x.ToString()).ToList());

            foreach (var trait in WeaponTrait.GetAll())
            {
                GameObject traitRow = Instantiate(weaponTraitEntryPrefab, weaponTraitParentPanel.transform);
                ((TMP_Text)traitRow.GetComponentInChildren(typeof(TMP_Text))).text = trait.Name;
                ((Toggle)traitRow.GetComponentInChildren(typeof(Toggle))).isOn = false;
                weaponTraits.Add(trait, traitRow.transform.GetComponentInChildren<Toggle>());

                UIOnHover uiOnHover = traitRow.AddComponent<UIOnHover>();
                uiOnHover.onHoverEnter += () =>
                {
                    weaponTraitDescription.text = trait.Description;
                };

                uiOnHover.onHoverExit += () =>
                {
                    weaponTraitDescription.text = "";
                };
            }

            IfWeight.text = "1";
            ActionManager.Instance.GetActions(LoadActionOptions, e => Debug.Log(e));
        }

        private void LoadActionOptions(List<ActionDto> actionDtos)
        {
            foreach (var actionDto in actionDtos)
            {
                Toggle toggle = Instantiate(ActionEntryPrefab, actionPanel.transform).GetComponent<Toggle>();
                toggle.isOn = false;
                toggle.transform.Find("Label").GetComponent<Text>().text = actionDto.Name;
                
                BaseAction customBaseAction = actionDto.MapToCustomAction();

                toggle.onValueChanged.AddListener(value => {
                    if (GetCurrentItemType() == ItemType.Consumable)
                    {
                        lastAction.Key.SetIsOnWithoutNotify(false);
                    }
                    if (value)
                    {
                        if (lastAction.Key != null && GetCurrentItemType() == ItemType.Consumable)
                        {
                            lastAction.Key.isOn = false;
                        }
                        lastAction = new KeyValuePair<Toggle, BaseAction>(toggle, customBaseAction);
                    }
                });

                UIOnHover uIOnHover = toggle.AddComponent<UIOnHover>();
                uIOnHover.onHoverEnter += () =>
                {
                    actionDescription.text = actionDto.Description;
                };

                uIOnHover.onHoverExit += () =>
                {
                    actionDescription.text = "";
                };
                actionToggles.Add(toggle, customBaseAction);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(actionPanel.transform.parent.GetComponent<RectTransform>());

            OnInitialized?.Invoke();
            IsInitialized = true;
        }

        private void CreateItem() {
            object item;
            switch (GetCurrentItemType())
            {
                case ItemType.Consumable:
                    if (actionToggles.Where(x => x.Key.isOn).Count() != 1) {
                        errorPanel.SetErrorMessage("A consumable must have 1 action");
                        return;
                    }
                    BaseConsumable baseConsumable = new()
                    {
                        Name = ifName.text,
                        ConsumeAction = actionToggles.FirstOrDefault(x => x.Key.isOn).Value
                    };
                    item = baseConsumable;

                    break;
                case ItemType.Weapon:
                    DamageInfo? damageInfo = weaponDamageEntry.GetDamageEntry();
                    if (damageInfo == null)
                    {
                        errorPanel.SetErrorMessage("Invalid damage info");
                        return;
                    }
                    BaseWeapon baseWeapon = new()
                    {
                        Name = ifName.text,
                        Description = ifDescription.text,
                        Weight = int.Parse(IfWeight.text)
                    };
                    foreach (var toggle in actionToggles.Where(x => x.Key.isOn))
                    {
                        baseWeapon.Actions.Add(toggle.Value);
                    }
                    foreach (var traitPair in weaponTraits.Where(x => x.Value.isOn))
                    {
                        baseWeapon.WeaponTraits.Add(traitPair.Key);
                    }
                    baseWeapon.DamageInfos = new List<DamageInfo>() { damageInfo };
                    if (baseWeapon.WeaponTraits.Contains(WeaponTrait.Heavy))
                    {
                        baseWeapon.WeaponSlotDetails = WeaponSlotDetails.TwoHand;
                    }
                    else
                    {
                        baseWeapon.WeaponSlotDetails = WeaponSlotDetails.OneHand;
                    }
                    baseWeapon.WeaponType = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ElementAt(weaponTypesDropdown.value);
                    baseWeapon.Actions.Add(new BasicMeleeWeaponAttack(baseWeapon, "Basic weapon attack"));
                    item = baseWeapon;
                    
                    break;
                default:
                    errorPanel.SetErrorMessage("Unknown item type");
                    return;
            }
            ItemDto itemDto = new()
            {
                Name = ifName.text,
                Type = GetCurrentItemType(),
                Description = ifDescription.text,
                Data = JsonConvert.SerializeObject(item, JsonSerializerSettingsProvider.GetSettings())
            };
            if (currentId == null) 
            {
                ItemManager.Instance.CreateItem(itemDto, (createdItem) => {
                    errorPanel.SetInfoMessage($"{createdItem.Name} was sucessfully created");
                }, errorPanel.SetErrorMessage);
            }
            else
            {
                itemDto.Id = currentId;
                ItemManager.Instance.UpdateItem(itemDto, () => { 
                    errorPanel.SetInfoMessage($"{itemDto.Name} was sucessfully updated");
                }, errorPanel.SetErrorMessage);
            }
            
        }

        public void LoadItem(ItemDto item) { 
            currentId = item.Id;
            ifName.text = item.Name;
            ifDescription.text = item.Description;

            switch (item.Type)
            {
                case ItemType.Consumable:
                    BaseConsumable loadedConsumable = JsonConvert.DeserializeObject<BaseConsumable>(item.Data, JsonSerializerSettingsProvider.GetSettings());
                    IfWeight.text = loadedConsumable.Weight.ToString();
                    

                    break;
                case ItemType.Weapon:
                    BaseWeapon loadedWeapon = JsonConvert.DeserializeObject<BaseWeapon>(item.Data, JsonSerializerSettingsProvider.GetSettings());
                    IfWeight.text = loadedWeapon.Weight.ToString();
                    break;
                default:
                    break;
            }
        }

        public void CleanUp()
        {
            currentId = null;

            actionToggles.Select(x => x.Key.isOn = false);

            weaponTraits.Select(x => x.Value.isOn = false);

            ifName.text = "";
            IfWeight.text = "";
            itemTypeDropdown.value = 0;
        }

        private ItemType GetCurrentItemType()
        {
            return Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ElementAt(itemTypeDropdown.value);
        }

        private void GoBack() { 
            parentPanel.SetActive(true);
            parentPanel.GetComponent<ItemList>().RefreshItems();
            gameObject.SetActive(false);
        }
    }
}
