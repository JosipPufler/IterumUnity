using Assets.DTOs;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Managers;
using Iterum.models.interfaces;
using Iterum.Scripts.Utils.Managers;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class ItemList : MonoBehaviour
    {
        public GameObject content;
        public GameObject entryPrefab;
        public ItemMaker itemMaker;
        public Button btnNewItem;
        public GameObject mainPanel;

        private ItemDto selectedItem;

        readonly IList<GameObject> entries = new List<GameObject>();

        private void Start()
        {
            btnNewItem.onClick.AddListener(() =>
            {
                if (itemMaker.IsInitialized)
                {
                    itemMaker.CleanUp();
                }
            });

            RefreshItems();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                mainPanel.SetActive(true);
            }
        }

        public void RefreshItems()
        {
            ItemManager.Instance.GetItems(ReloadItems, OnError);
        }

        void ReloadItems(IEnumerable<ItemDto> characters)
        {
            foreach (var item in entries)
            {
                Destroy(item);
            }

            foreach (ItemDto character in characters)
            {
                AddEntry(character);
            }
        }

        void AddEntry(ItemDto item)
        {
            var entry = Instantiate(entryPrefab, content.transform);

            entry.transform.Find("lblName").GetComponent<TMP_Text>().text = item.Name;
            entry.transform.Find("lblType").GetComponent<TMP_Text>().text = item.Type.ToString();
            var buttonHolder = entry.transform.Find("buttons");
            buttonHolder.Find("btnEdit").GetComponent<Button>().onClick.AddListener(() => LoadItem(item));
            buttonHolder.Find("btnDelete").GetComponent<Button>().onClick.AddListener(() => DeleteItem(item, entry.transform));

            entry.SetActive(true);

            entries.Add(entry);
        }

        void LoadItem(ItemDto itemDto)
        {
            mainPanel.SetActive(false);
            itemMaker.gameObject.SetActive(true);
            selectedItem = itemDto;
            if (itemMaker.IsInitialized)
            {
                itemMaker.LoadItem(selectedItem);
            }
            else
                itemMaker.OnInitialized += OnInitializedLoadCharacter;
            gameObject.SetActive(false);
        }

        void OnInitializedLoadCharacter()
        {
            itemMaker.LoadItem(selectedItem);
            itemMaker.OnInitialized -= OnInitializedLoadCharacter;
        }

        void DeleteItem(ItemDto itemDto, Transform transform)
        {
            ItemManager.Instance.DeleteItem(itemDto.Id, () => {
                ItemManager.Instance.GetItems(ReloadItems, OnError);
                Destroy(transform.gameObject);
            }, OnError);
        }

        void OnError(string error)
        {
            Debug.Log(error);
        }
    }
}
