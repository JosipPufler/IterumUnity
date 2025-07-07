using UnityEngine;
using Random = System.Random;
using Iterum.models.interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts.Utils;
using Iterum.models.creatures;
using Assets.Scripts.GameLogic.models.creatures;
using Iterum.Scripts.UI;
using System;
using TMPro;

public class GeneralManager : MonoBehaviour
{
    [Header("Managers")]
    public ScrollViewAutoCenter initiativeBar;
    public CampaignGridLayout layoutManager;
    public CampaignMenuManager menuManager;

    [Header("Prefabs")]
    public GameObject initiativePortraitPrefab;

    [Header("Combat host tab")]
    public Button btnStartCombat;
    public GameObject creatureEntryPrefab;
    public GameObject content;
    public CampaignMenuManager actionManager;

    private GameObject initiativeBarContent;
    private readonly Random rng = new();

    List<(int priority, ICreature creature)> initiativeOrder = new();
    readonly List<(GameObject portrait, ICreature creature)> portraitOrder = new();
    private readonly List<Type> creatures = new() { typeof(Wolf), typeof(AlphaWolf)};

    private bool InCombat = false;

    private void Start()
    {
        initiativeBarContent = initiativeBar.content.gameObject;
        foreach (var creature in creatures) {
            var entry = Instantiate(creatureEntryPrefab, content.transform);
            entry.transform.Find("Name").GetComponent<TMP_Text>().text = StaticUtils.GetDisplayName(creature);
            entry.GetComponent<Button>().onClick.AddListener(() => {
                if (GameManager.Instance.SelectedCreature == creature)
                {
                    GameManager.Instance.SelectedCreature = null;
                }
                else 
                {
                    GameManager.Instance.SelectedCreature = creature;
                }
            });
        }

        btnStartCombat.onClick.AddListener(StartCombat);
    }

    public void StartCombat() {
        InCombat = true;
        foreach (Transform child in initiativeBarContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (ICreature character in layoutManager.GetCombatants())
        {
            initiativeOrder.Add((character.RollInitiative(), character));
        }

        initiativeOrder = initiativeOrder
            .GroupBy(x => x.priority)
            .OrderByDescending(g => g.Key)
            .SelectMany(g => g.OrderBy(_ => rng.Next()))
            .ToList();

        foreach (var (priority, creature) in initiativeOrder)
        {
            portraitOrder.Add((CreatePortrait(creature), creature));
        }

        (GameObject portrait, ICreature creature) value = portraitOrder.ElementAt(0);
        value.portrait.GetComponent<Outline>().effectColor = Color.white;
        initiativeBar.UpdateScroll();
        menuManager.SetCreature(value.creature);
        actionManager.SetCreature(value.creature);
    }

    private GameObject CreatePortrait(ICreature creature)
    {
        var entry = Instantiate(initiativePortraitPrefab, initiativeBarContent.transform);

        entry.transform.Find("CharPortrait").GetComponent<RawImage>().texture = TextureMemorizer.textures[creature.ImagePath];

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

    public void UpdateInitiative(ICreature creature) {
        if (!InCombat)
        {
            return;
        }

        int roll = creature.RollInitiative();
        var entry = (roll, creature);

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
        portraitOrder.Insert(insertIndex, (CreatePortrait(creature), creature));
    }

    public void EndTurn() {
        if (initiativeOrder.Count == 0)
        {
            return;
        }
        initiativeOrder.RemoveAt(0);
        Destroy(portraitOrder.ElementAt(0).portrait);
        portraitOrder.RemoveAt(0);
        initiativeBar.UpdateScroll();
    }
}