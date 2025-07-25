using Assets.Scripts.Campaign;
using Assets.Scripts.Utils;
using Iterum.models.interfaces;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class GeneralManager : MonoBehaviour
{
    public static GeneralManager Instance;

    private void Awake() => Instance = this;

    [Header("Managers")]
    public ScrollViewAutoCenter initiativeBar;
    public CampaignGridLayout layoutManager;
    public CampaignMenuManager menuManager;

    [Header("Prefabs")]
    public GameObject initiativePortraitPrefab;

    [Header("Controls")]
    public Button btnEndTurn;

    private GameObject initiativeBarContent;

    public List<(int priority, CharacterToken token)> initiativeOrder = new();
    readonly List<(GameObject portrait, CharacterToken token)> portraitOrder = new();

    public bool InCombat = false;

    private void Start()
    {
        btnEndTurn.onClick.AddListener(OnEndTurnButtonClicked);

        initiativeBarContent = initiativeBar.content.gameObject;
    }

    /*private void Update()
    {
        for (int i = initiativeOrder.Count-1; i >= 0; i--)
        {
            if (initiativeOrder.ElementAt(i).token.creature.IsDead)
            {
                initiativeOrder.RemoveAt(i);
                Destroy(portraitOrder.ElementAt(i).portrait);
                portraitOrder.RemoveAt(i);
                if (i == 0)
                {
                    HighlightFirst();
                }
            }
        }
    }*/

    /*public void StartCombatTurn(bool first)
    {
        InCombat = true;
        foreach (Transform child in initiativeBarContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (CharacterToken token in layoutManager.GetCombatants())
        {
            initiativeOrder.Add((token.creature.RollInitiative(), token));
            if (first)
                token.creature.CurrentAp = token.creature.MaxAp;
        }

        initiativeOrder = initiativeOrder
            .GroupBy(x => x.priority)
            .OrderByDescending(g => g.Key)
            .SelectMany(g => g.OrderBy(_ => rng.Next()))
            .ToList();

        foreach (var (priority, token) in initiativeOrder)
        {
            portraitOrder.Add((CreatePortrait(token), token));
        }

        HighlightFirst();
    }*/

    private void HighlightFirst()
    {
        if (portraitOrder.Count == 0)
        {
            return;
        }

        (GameObject portrait, CharacterToken token) value = portraitOrder.ElementAt(0);
        value.portrait.GetComponent<Outline>().effectColor = Color.white;
        initiativeBar.UpdateScroll();
        if (value.token.isOwned)
        {
            btnEndTurn.gameObject.SetActive(true);
            menuManager.SetCreature(value.token);
        }
        else
        {
            btnEndTurn.gameObject.SetActive(false);
        }
        value.token.creature.StartTurn();
    }

    private GameObject CreatePortrait(CharacterToken token)
    {
        BaseCreature creature = token.creature;
        var entry = Instantiate(initiativePortraitPrefab, initiativeBarContent.transform);

        entry.transform.Find("CharPortrait").GetComponent<RawImage>().texture = TextureMemorizer.textures[creature.ImagePath];
        entry.GetComponent<CharacterPortrait>().CharacterToken = token;

        Outline outline = entry.GetComponent<Outline>();

        switch (layoutManager.GetCreatureTeam(creature.ID))
        {
            case Assets.Scripts.GameLogic.models.enums.Team.PLAYER:
                outline.effectColor = Color.green;
                break;
            case Assets.Scripts.GameLogic.models.enums.Team.ALLY:
                outline.effectColor = Color.blue;
                break;
            case Assets.Scripts.GameLogic.models.enums.Team.ENEMY:
                outline.effectColor = Color.red;
                break;
            case Assets.Scripts.GameLogic.models.enums.Team.NEUTRAL:
                outline.effectColor = Color.yellow;
                break;
            case Assets.Scripts.GameLogic.models.enums.Team.DEAD:
                outline.effectColor = Color.black;
                break;
        }
        return entry;
    }

    public void RemovePortrait(CharacterToken token)
    {
        for (int i = 0; i < portraitOrder.Count; i++)
        {
            if (portraitOrder[i].token == token)
            {
                Destroy(portraitOrder[i].portrait);
                portraitOrder.RemoveAt(i);
                break;
            }
        }
    }

    /*public void UpdateInitiative(CharacterToken token) {
        if (!InCombat)
        {
            return;
        }

        int roll = token.creature.RollInitiative();
        var entry = (roll, token);

        int insertIndex = initiativeOrder.Count;

        for (int i = 0; i < initiativeOrder.Count; i++)
        {
            if (initiativeOrder[i].priority < roll)
            {
                insertIndex = i;
                break;
            }
            if (initiativeOrder[i].priority == roll)
            {
                insertIndex = i + 1;
            }
        }

        initiativeOrder.Insert(insertIndex, entry);
        portraitOrder.Insert(insertIndex, (CreatePortrait(token), token));
    }*/

    /*public void EndTurn() {
        if (initiativeOrder.Count == 0)
        {
            return;
        }
        initiativeOrder.ElementAt(0).token.creature.EndTurn();
        initiativeOrder.RemoveAt(0);
        Destroy(portraitOrder.ElementAt(0).portrait);
        portraitOrder.RemoveAt(0);
        initiativeBar.UpdateScroll();

        if (initiativeOrder.Count == 0)
        {
            StartCombatTurn(false);
        }
        else
        {
            HighlightFirst();
        }
    }*/

    public void SyncPortraitsWithOrder(IList<CharacterToken> tokens)
    {
        // Clear current UI
        foreach (Transform child in initiativeBar.content.transform)
            Destroy(child.gameObject);

        portraitOrder.Clear();
        foreach (var token in tokens)
        {
            var portrait = CreatePortrait(token);
            portraitOrder.Add((portrait, token));
        }

        HighlightFirst();
    }

    public void OnEndTurnButtonClicked()
    {
        if (NetworkClient.localPlayer is NetworkIdentity identity)
            identity.connectionToServer.identity.GetComponent<CampaignPlayer>().CmdRequestEndTurn();
    }

    public CharacterToken GetCurrentToken() {
        if (portraitOrder == null || portraitOrder.Count == 0)
        {
            return null;
        }
        return portraitOrder.First().token;
    }
}