using UnityEngine;
using Random = System.Random;
using Iterum.models.interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.Campaign;

public class GeneralManager : MonoBehaviour
{
    [Header("Managers")]
    public ScrollViewAutoCenter initiativeBar;
    public CampaignGridLayout layoutManager;
    public CampaignMenuManager menuManager;
    public CameraController cameraController;

    [Header("Prefabs")]
    public GameObject initiativePortraitPrefab;

    [Header("Controls")]
    public Button btnEndTurn;

    private GameObject initiativeBarContent;
    private readonly Random rng = new();

    public List<(int priority, CharacterToken token)> initiativeOrder = new();
    readonly List<(GameObject portrait, CharacterToken token)> portraitOrder = new();

    public bool InCombat = false;

    private void Start()
    {
        btnEndTurn.onClick.AddListener(EndTurn);

        initiativeBarContent = initiativeBar.content.gameObject;
    }

    private void Update()
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
    }

    public void StartCombatTurn(bool first)
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
    }

    private void HighlightFirst()
    {
        if (portraitOrder.Count == 0)
        {
            return;
        }

        (GameObject portrait, CharacterToken token) value = portraitOrder.ElementAt(0);
        value.portrait.GetComponent<Outline>().effectColor = Color.white;
        initiativeBar.UpdateScroll();
        menuManager.SetCreature(value.token);
        value.token.creature.StartTurn();
    }

    private GameObject CreatePortrait(CharacterToken token)
    {
        ICreature creature = token.creature;
        var entry = Instantiate(initiativePortraitPrefab, initiativeBarContent.transform);

        entry.transform.Find("CharPortrait").GetComponent<RawImage>().texture = TextureMemorizer.textures[creature.ImagePath];
        entry.GetComponent<CharacterPortrait>().CharacterToken = token;
        entry.GetComponent<CharacterPortrait>().CameraRig = cameraController;

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

    public void UpdateInitiative(CharacterToken token) {
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
    }

    public void EndTurn() {
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
    }

    public CharacterToken GetCurrentToken() {
        return initiativeOrder.First().token;
    }
}