using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.Map;
using Iterum.DTOs;
using Iterum.models.interfaces;
using Iterum.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CampaignGridLayout : HexGridLayout
{
    public GeneralManager manager;

    [Header("Token")]
    public GameObject tokenPrefab;
    public Material allyMaterial;
    public Material neutralMaterial;
    public Material enemyMaterial;
    public Material playerMaterial;
    public Material deadMaterial;

    private readonly List<CharacterToken> players = new();
    private readonly List<CharacterToken> neutral = new();
    private readonly List<CharacterToken> allies = new();
    private readonly List<CharacterToken> enemies = new();

    private readonly Dictionary<Vector3Int, CharacterToken> characters = new();

    private void Update()
    {
        // Place on top
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("Hex"))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f)
                    {
                        foreach (var pair in grid)
                        {
                            if (pair.Value == hitObject)
                            {
                                Vector3Int currentKey = pair.Key;
                                Vector3Int aboveKey = new Vector3Int(currentKey.x, currentKey.y + 1, currentKey.z);
                                AddToken(aboveKey);
                                break;
                            }
                        }
                    }
                }

                if (hitObject.CompareTag("Token"))
                {
                    CharacterToken characterToken = hitObject.GetComponent<CharacterToken>();
                    characterToken.Die();
                }
            }
        }

        // Clear selected
        if ((Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) || Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.SelectedCreature = null;
        }
    }

    private void AddToken(Vector3Int aboveKey)
    {
        if (characters.TryGetValue(aboveKey, out CharacterToken _))
        {
            return;
        }

        if (GameManager.Instance.SelectedCreature == null)
        {
            return;
        }
        Material currentMaterial = null;

        var token = Instantiate(tokenPrefab, transform);
        CharacterToken characterToken = token.GetComponent<CharacterToken>();
        switch (GameManager.Instance.Team)
        {
            case Team.PLAYER:
                currentMaterial = playerMaterial;
                players.Add(characterToken);
                break;
            case Team.ALLY:
                currentMaterial = allyMaterial;
                allies.Add(characterToken);
                break;
            case Team.ENEMY:
                currentMaterial = enemyMaterial;
                enemies.Add(characterToken);
                break;
            case Team.NEUTRAL:
                currentMaterial = neutralMaterial;
                neutral.Add(characterToken);
                break;
            case Team.DEAD:
                currentMaterial = deadMaterial;
                break;
        }
        token.transform.Find("body").GetComponent<MeshRenderer>().material = currentMaterial;
        token.transform.localScale = new Vector3(1.5f, 1f, 1.6f);
        float bodyHeight = token.transform.Find("outlineBody").GetComponent<MeshRenderer>().bounds.size.y;

        Vector3 vector3 = GetPositionForHexFromCoordinate(new Vector2Int(aboveKey.x, aboveKey.z));
        vector3.y = aboveKey.y * height * 2 + (bodyHeight / 2f) + 1;
        token.transform.position = vector3;

        token.tag = "Token";

        ICreature creature = (ICreature)Activator.CreateInstance(GameManager.Instance.SelectedCreature);

        if (!token.TryGetComponent<Collider>(out _))
        {
            BoxCollider boxCollider = token.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(100, 30, 100);
        }

        characterToken.SetCreature(creature);
        characterToken.normalBodyMaterial = currentMaterial;
        manager.UpdateInitiative(creature);

        characters[aboveKey] = characterToken;
    }

    public void LoadMap(MapDto mapDto) {
        foreach (var hex in grid.Values)
        {
            Destroy(hex);
        }

        grid.Clear();

        isFlatTopped = mapDto.IsFlatTopped;
        gridSize.x = mapDto.maxX;
        gridSize.y = mapDto.maxY;

        foreach (var hex in mapDto.Hexes)
        {
            TryAddHex(new Vector3Int(hex.X, hex.Y, hex.Z));
        }
    }

    protected override void TryAddHex(Vector3Int key)
    {
        if (!grid.TryGetValue(key, out GameObject hex))
        {
            GameObject tile = new($"Hex {key.x},{key.y}, ", typeof(HexRenderer));
            tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(key.x, key.z));
            tile.tag = "Hex";

            HexRenderer r = tile.GetComponent<HexRenderer>();
            if (key.y == 0)
                r.Initialize(hexMaterial, 0, outerSize-5, height * 2 + 1000, isFlatTopped, -1000);
            else
                r.Initialize(hexMaterial, 0, outerSize-5, height * 2, isFlatTopped, key.y * height * 2);

            tile.transform.SetParent(transform, true);
            grid[key] = tile;
        }
    }

    public List<ICreature> GetCombatants() {
        return players.Select(x => x.creature)
              .Concat(enemies.Select(x => x.creature))
              .Concat(allies.Select(x => x.creature))
              .Concat(neutral.Select(x => x.creature))
              .ToList();
    }

    public Team GetCreatureTeam(string creatureId)
    {
        if (enemies.Any(x => x.creature.ID == creatureId))
        {
            return Team.ENEMY;
        }
        if (allies.Any(x => x.creature.ID == creatureId))
        {
            return Team.ALLY;
        }
        if (neutral.Any(x => x.creature.ID == creatureId))
        {
            return Team.NEUTRAL;
        }
        if (players.Any(x => x.creature.ID == creatureId))
        {
            return Team.PLAYER;
        }
        return Team.DEAD;
    }
}
