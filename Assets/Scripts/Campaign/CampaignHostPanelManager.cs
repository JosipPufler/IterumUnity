using Assets.DTOs;
using Assets.Scripts.GameLogic.models.creatures;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Managers;
using Iterum.DTOs;
using Iterum.models.creatures;
using Iterum.models.interfaces;
using Iterum.Scripts.UI;
using Iterum.Scripts.Utils;
using Iterum.Scripts.Utils.Managers;
using Mirror.Examples.CharacterSelection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignHostPanelManager : MonoBehaviour
{
    [Header("Map")]
    public GameObject mapContent;
    public GameObject mapEntryTemplate;
    public CampaignGridLayout campaignGridLayout;

    [Header("Assets")]
    public GameObject imageContent;
    public GameObject journalContent;
    public GameObject assetPrefab;

    [Header("Chat")]
    public GameObject chatContent;
    public GameObject chatLinkPrefab;

    [Header("Preview")]
    public RawImage imagePreview;
    public TMP_Text journalPreviewContent;
    public GameObject journalPreviewPanel;

    [Header("Combat")]
    public TMP_Dropdown dropdown;
    public Button btnStartCombat;
    public GameObject creatureEntryPrefab;
    public GameObject content;
    public GeneralManager generalManager;

    readonly List<Type> creatures = new() { typeof(Wolf), typeof(AlphaWolf) };

    void Start()
    {
        dropdown.ClearOptions();

        var options = new List<string>(Enum.GetNames(typeof(Team)));
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(value => {
            GameManager.Instance.Team = (Team)value;
        });

        foreach (var creature in creatures)
        {
            var entry = Instantiate(creatureEntryPrefab, content.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = StaticUtils.GetDisplayName(creature);
            entry.GetComponent<Button>().onClick.AddListener(() => {
                if (GameManager.Instance.SelectedCreature != null && GameManager.Instance.SelectedCreature.GetType() == creature)
                {
                    GameManager.Instance.SelectedCreature = null;
                }
                else
                {
                    GameManager.Instance.SelectedCreature = (ICreature)Activator.CreateInstance(creature);
                }
            });
        }

        CharacterManager.Instance.GetCharacters(PopulateCustomCharacters, OnError);

        btnStartCombat.onClick.AddListener(() => generalManager.StartCombatTurn(true));

        FetchAll();
    }

    void PopulateCustomCharacters(List<CharacterDto> characterDtos) {
        foreach (var characterDto in characterDtos)
        {
            ICreature character = characterDto.MapToCreature();
            var entry = Instantiate(creatureEntryPrefab, content.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = character.Name;
            entry.GetComponent<Button>().onClick.AddListener(() => {
                if (GameManager.Instance.SelectedCreature != null && GameManager.Instance.SelectedCreature.Name == character.Name)
                {
                    GameManager.Instance.SelectedCreature = null;
                }
                else
                {
                    GameManager.Instance.SelectedCreature = character;
                }
            });
        }

    }

    void FetchAll() {
        MapManager.Instance.GetMaps(PopulateMaps, OnError);
        AssetManager.Instance.GetImages(PopulateImages, OnError);
        JournalManager.Instance.GetJournals(PopulateJournals, OnError);
    }

    void PopulateMaps(List<MapDto> maps) {
        foreach (var map in maps)
        {
            var entry = Instantiate(mapEntryTemplate, mapContent.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = map.Name;

            entry.transform.Find("btnLoad").GetComponent<Button>()
                .onClick.AddListener(() => LoadMap(map));
            
            entry.SetActive(true);
        }
    }

    void PopulateImages(List<string> imageNames) {
        foreach (var image in imageNames)
        {
            var entry = Instantiate(assetPrefab, imageContent.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = image;

            entry.transform.Find("btnShare").GetComponent<Button>()
                .onClick.AddListener(() => {
                    var chatEntry = Instantiate(chatLinkPrefab, chatContent.transform);
                    chatEntry.transform.Find("Name").GetComponent<TMP_Text>().text = image;
                    chatEntry.GetComponent<Button>().onClick.AddListener(() => {
                        AssetManager.Instance.GetImage(image, PreviewImage, OnError);
                    });
                });

            entry.transform.Find("btnPreview").GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    AssetManager.Instance.GetImage(image, PreviewImage, OnError);
                });

            entry.SetActive(true);
        }
    }

    private void PreviewImage(Texture2D texture)
    {
        imagePreview.texture = texture;

        float maxWidth = 1920f;
        float maxHeight = 1080f;

        float imageWidth = texture.width;
        float imageHeight = texture.height;

        float widthRatio = maxWidth / imageWidth;
        float heightRatio = maxHeight / imageHeight;

        float scale = Mathf.Min(widthRatio, heightRatio);

        float width = imageWidth * scale;
        float height = imageHeight * scale;

        RectTransform rectTransform = imagePreview.rectTransform;
        rectTransform.sizeDelta = new Vector2(width, height);

        imagePreview.gameObject.SetActive(true);
    }

    void PopulateJournals(IEnumerable<string> journalNames)
    {
        foreach (var journal in journalNames)
        {
            var entry = Instantiate(assetPrefab, journalContent.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = journal;

            entry.transform.Find("btnShare").GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    var chatEntry = Instantiate(chatLinkPrefab, chatContent.transform);
                    chatEntry.transform.Find("Name").GetComponent<TMP_Text>().text = journal;
                    chatEntry.GetComponent<Button>().onClick.AddListener(() => {
                        JournalManager.Instance.PreviewJournal($"{PlayerPrefs.GetString("username")}/journal/{name}.txt", PreviewJournal, OnError);
                    });
                });

            entry.transform.Find("btnPreview").GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    JournalManager.Instance.GetJournal(journal, PreviewJournal, OnError);
                });
        }
    }
    
    private void PreviewJournal(JournalDto JournalDto)
    {
        journalPreviewContent.text = JournalDto.Content;
        journalPreviewPanel.SetActive(true);
    }

    void LoadMap(MapDto mapDto) { 
        campaignGridLayout.LoadMap(mapDto);
    }

    void OnError(string error) {
        Debug.Log(error);
    }
}
