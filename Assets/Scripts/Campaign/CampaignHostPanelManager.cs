using Assets.DTOs;
using Assets.Scripts.Campaign;
using Assets.Scripts.GameLogic.models.creatures;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.converters;
using Assets.Scripts.Utils.Managers;
using Iterum.DTOs;
using Iterum.models.creatures;
using Iterum.models.interfaces;
using Iterum.Scripts.UI;
using Iterum.Scripts.Utils;
using Iterum.Scripts.Utils.Managers;
using Mirror;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignHostPanelManager : MonoBehaviour
{
    [Header("Objects")]
    public Button hostPanelButton;
    public GameObject hostPanel;

    [Header("Map")]
    public GameObject mapContent;
    public GameObject mapEntryTemplate;
    public CampaignGridLayout campaignGridLayout;
    public NetworkCampaignGrid networkGrid;

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

    [Header("Player")]
    public GameObject playerPanel;
    public GameObject playerPanelCharacters;

    private CampaignPlayer campaignPlayer;

    readonly List<Type> creatures = new() { typeof(Wolf), typeof(AlphaWolf) };

    private void Start()
    {
        hostPanelButton.onClick.AddListener(() => {
            if (campaignPlayer.isCampaignHost)
            {
                hostPanel.SetActive(true);
            }
            else
            {
                playerPanel.SetActive(true);
            }
        });
        hostPanel.SetActive(false);
    }

    void InitialiseUI()
    {
        dropdown.ClearOptions();

        var options = new List<string>(Enum.GetNames(typeof(Team)));
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(v => GameManager.Instance.Team = (Team)v);

        foreach (var creature in creatures)
        {
            var entry = Instantiate(creatureEntryPrefab, content.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = StaticUtils.GetDisplayName(creature);
            entry.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (GameManager.Instance.SelectedCreature != null &&
                    GameManager.Instance.SelectedCreature.GetType() == creature)
                    GameManager.Instance.SelectedCreature = null;
                else
                {
                    GameManager.Instance.SelectedCreature = (BaseCreature)Activator.CreateInstance(creature);
                    GameManager.Instance.SelectedCharacter = null;
                }
            });
        }
        
        CharacterManager.Instance.GetCharacters(PopulateCustomCharacters, OnError);
        btnStartCombat.onClick.AddListener(OnStartCombatClicked);

        FetchAll();
    }

    public void OnStartCombatClicked()
    {
        if (NetworkClient.localPlayer is NetworkIdentity identity)
            identity.connectionToServer.identity.GetComponent<CampaignPlayer>().CmdRequestStartCombat();
    }

    public void AttachToLocalPlayer(CampaignPlayer player)
    {
        campaignPlayer = player;
        hostPanel.SetActive(false);
        playerPanel.SetActive(false);
        if (!player.isCampaignHost)
        {
            if (GameManager.Instance.SelectedCharacter != null && ConverterUtils.TryParseCreature(GameManager.Instance.SelectedCharacter.Data, out BaseCreature creature))
            {
                var character = GameManager.Instance.SelectedCharacter;
                var entry = Instantiate(creatureEntryPrefab, playerPanelCharacters.transform);
                entry.transform.Find("Name").GetComponent<TMP_Text>().text = creature.Name;
                entry.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (GameManager.Instance.SelectedCreature != null && GameManager.Instance.SelectedCreature.Name == creature.Name)
                    {
                        GameManager.Instance.SelectedCreature = null;
                    }
                    else
                    {
                        GameManager.Instance.SelectedCreature = creature;
                    }
                });
            }
        }
        else 
        {
            InitialiseUI();
        }
    }

    void PopulateCustomCharacters(List<CharacterDto> characterDtos) {
        foreach (var characterDto in characterDtos)
        {
            var entry = Instantiate(creatureEntryPrefab, content.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = characterDto.Name;
            entry.GetComponent<Button>().onClick.AddListener(() => {
                if (GameManager.Instance.SelectedCharacter != null && GameManager.Instance.SelectedCharacter.Name == characterDto.Name)
                {
                    GameManager.Instance.SelectedCharacter = null;
                }
                else
                {
                    GameManager.Instance.SelectedCharacter = characterDto;
                    GameManager.Instance.SelectedCreature = null;
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
                campaignPlayer.CmdSendChatLinkEntry(image, "image");
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
            .onClick.AddListener(() => {
                campaignPlayer.CmdSendChatLinkEntry(journal, "journal");
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
        networkGrid.CmdLoadMapJson(JsonConvert.SerializeObject(mapDto));
    }

    void OnError(string error) {
        Debug.Log(error);
    }

    public void AddChatLinkEntry(string label, string type)
    {
        var chatEntry = Instantiate(chatLinkPrefab, chatContent.transform);
        chatEntry.transform.Find("Name").GetComponent<TMP_Text>().text = label;

        if (type == "image")
        {
            chatEntry.GetComponent<Button>().onClick.AddListener(() => {
                AssetManager.Instance.GetImage(label, PreviewImage, OnError);
            });
        }
        else if (type == "journal")
        {
            chatEntry.GetComponent<Button>().onClick.AddListener(() => {
                JournalManager.Instance.PreviewJournal($"{PlayerPrefs.GetString("username")}/journal/{label}.txt", PreviewJournal, OnError);
            });
        }
    }
}
