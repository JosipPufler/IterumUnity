using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Campaign;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.Map;
using Iterum.DTOs;
using Iterum.models.interfaces;
using Iterum.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CampaignGridLayout : HexGridLayout
{
    private const string TokenTag = "Token";
    private const string HexTag = "Hex";
    public GeneralManager manager;
    public CampaignActionManager actionManager;

    [Header("Token")]
    public GameObject tokenPrefab;
    public Material allyMaterial;
    public Material neutralMaterial;
    public Material enemyMaterial;
    public Material playerMaterial;
    public Material deadMaterial;

    private readonly Dictionary<Team, List<CharacterToken>> creatures = new();

    private static readonly Dictionary<Vector3Int, CharacterToken> characters = new();
    private CharacterToken selectedToken;

    private readonly Dictionary<Vector3Int, HexRenderer> selectedHexes = new();
    private bool movin = false;
    private int currentMoveNumber = 0;
    private Vector3Int lastHex;

    public static List<HexRenderer> highlightedRenderers = new();
    public static List<CharacterToken> highlightedCharacters = new();

    private void Start()
    {
        foreach (Team team in Enum.GetValues(typeof(Team)))
        {
            creatures[team] = new List<CharacterToken>();
        }
    }

    private void Update()
    {
        // Place on top
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag(HexTag))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f)
                    {
                        foreach (var pair in grid)
                        {
                            if (pair.Value == hitObject)
                            {
                                if (actionManager.LookingForTargets && IsWithinRange(GetTokenCoordinates(actionManager.CurrentToken), pair.Key, actionManager.CurrentTargetData.MinDistance, actionManager.CurrentTargetData.MaxDistance)) {
                                    actionManager.SubmitTargetData(new TargetDataSubmissionHex(actionManager.CurrentTargetData, pair.Key));
                                }
                                Vector3Int aboveKey = new(pair.Key.x, pair.Key.y + 1, pair.Key.z);
                                TryAddToken(aboveKey);
                                break;
                            }
                        }
                    }
                }

                if (hitObject.CompareTag(TokenTag))
                {
                    CharacterToken characterToken = hitObject.GetComponent<CharacterToken>();
                    Vector3Int tokenCoords = GetTokenCoordinates(characterToken);

                    if (IsTokenCurrentTurn(characterToken))
                    {
                        selectedToken = characterToken;
                        movin = true;
                        tokenCoords.y -= 1;
                        lastHex = tokenCoords;
                    }

                    if (actionManager.LookingForTargets && IsWithinRange(GetTokenCoordinates(actionManager.CurrentToken), tokenCoords, actionManager.CurrentTargetData.MinDistance, actionManager.CurrentTargetData.MaxDistance))
                    {
                        actionManager.SubmitTargetData(new TargetDataSubmissionCreature(actionManager.CurrentTargetData, characterToken.creature));
                        RotateTokenToward(manager.GetCurrentToken(), tokenCoords);
                    }
                }
            }
        }

        // Drag on top
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag(HexTag))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f)
                    {
                        foreach (var pair in grid)
                        {
                            if (pair.Value == hitObject)
                            {
                                Vector3Int currentKey = pair.Key;
                                if (movin)
                                {
                                    if (currentMoveNumber == selectedToken.creature.EffectiveMovementPoints || selectedHexes.ContainsKey(currentKey))
                                    {
                                        break;
                                    }
                                    if (!AreAdjacent(lastHex, currentKey))
                                    {
                                        Debug.Log("Not close");
                                        break;
                                    }
                                    if (selectedToken == null || !IsTokenCurrentTurn(selectedToken))
                                    {
                                        ResetMovement();
                                        break;
                                    }
                                    lastHex = currentKey;
                                    currentMoveNumber++;
                                    HexRenderer hexRenderer = pair.Value.GetComponent<HexRenderer>();
                                    hexRenderer.ShowHighlight(currentMoveNumber);
                                    selectedHexes.TryAdd(currentKey, hexRenderer);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Clear selected
        if ((Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) || Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.SelectedCreature = null;
            selectedToken = null;
            ResetMovement();
        }

        if (Input.GetMouseButtonUp(0) && movin) {
            foreach (Vector3Int hex in selectedHexes.Keys)
            {
                var aboveHex = hex;
                aboveHex.y += 1;
                RotateTokenToward(selectedToken, aboveHex);
                MoveToken(selectedToken, aboveHex);
            }
            
            ResetMovement();
        }
    }

    private bool IsTokenCurrentTurn(CharacterToken characterToken)
    {
        if (manager.initiativeOrder == null || manager.initiativeOrder.Count == 0)
        {
            return false;
        }
        return manager.initiativeOrder.First().token.creature == characterToken.creature;
    }

    private void ResetMovement()
    {
        movin = false;
        foreach (HexRenderer renderer in selectedHexes.Values)
        {
            renderer.HideHighlight();
        }
        selectedHexes.Clear();
        currentMoveNumber = 0;
    }

    private void MoveToken(CharacterToken selectedToken, Vector3Int aboveKey)
    {
        if (characters.TryGetValue(aboveKey, out CharacterToken _))
        {
            return;
        }
        Vector3Int from = GetTokenCoordinates(selectedToken);
        characters.Remove(from);
        characters[aboveKey] = selectedToken;
        selectedToken.MoveToken(GetPositionForTokenInRealWorld(aboveKey, selectedToken.gameObject));
    }

    private static Vector3Int GetTokenCoordinates(CharacterToken selectedToken)
    {
        return characters.First(x => x.Value == selectedToken).Key;
    }

    private void TryAddToken(Vector3Int aboveKey)
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
        creatures[GameManager.Instance.Team].Add(characterToken);
        switch (GameManager.Instance.Team)
        {
            case Team.PLAYER:
                currentMaterial = playerMaterial;
                characterToken.outlineColor = Color.blue;
                break;
            case Team.ALLY:
                currentMaterial = allyMaterial;
                characterToken.outlineColor = Color.green;
                break;
            case Team.ENEMY:
                currentMaterial = enemyMaterial;
                characterToken.outlineColor = Color.red;
                break;
            case Team.NEUTRAL:
                currentMaterial = neutralMaterial;
                characterToken.outlineColor = Color.white;
                break;
            case Team.DEAD:
                currentMaterial = deadMaterial;
                characterToken.outlineColor = Color.white;
                break;
        }
        token.transform.Find("body").GetComponent<MeshRenderer>().material = currentMaterial;
        token.transform.localScale = new Vector3(1.6f, 1f, 1.6f);
        Vector3 vector3 = GetPositionForTokenInRealWorld(aboveKey, token);
        token.transform.position = vector3;

        token.tag = "Token";

        if (!token.TryGetComponent<Collider>(out _))
        {
            BoxCollider boxCollider = token.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(100, 30, 100);
        }

        characterToken.SetCreature(GameManager.Instance.SelectedCreature);
        characterToken.normalBodyMaterial = currentMaterial;
        manager.UpdateInitiative(characterToken);

        characters[aboveKey] = characterToken;
    }

    private Vector3 GetPositionForTokenInRealWorld(Vector3Int aboveKey, GameObject token)
    {
        float bodyHeight = token.transform.Find("outlineBody").GetComponent<MeshRenderer>().bounds.size.y;

        Vector3 vector3 = GetPositionForHexFromCoordinate(new Vector2Int(aboveKey.x, aboveKey.z));
        vector3.y = aboveKey.y * height * 2 + (bodyHeight / 2f) + 1;
        return vector3;
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
        if (!grid.ContainsKey(key))
        {
            GameObject tile = new($"Hex {key.x},{key.y}, ", typeof(HexRenderer));
            tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(key.x, key.z));
            tile.tag = "Hex";

            HexRenderer r = tile.GetComponent<HexRenderer>();
            if (key.y == 0)
                r.Initialize(hexMaterial, highlightMaterial, targetMaterial, 0, outerSize-5, height * 2 + 1000, isFlatTopped, -1000);
            else
                r.Initialize(hexMaterial, highlightMaterial, targetMaterial, 0, outerSize-5, height * 2, isFlatTopped, key.y * height * 2);

            tile.transform.SetParent(transform, true);
            grid[key] = tile;
        }
    }

    public List<CharacterToken> GetCombatants() {
        List<CharacterToken> allCreatures = new();
        foreach (Team team in creatures.Keys)
        {
            allCreatures.AddRange(creatures[team].Where(x => !x.creature.IsDead));
        }
        return allCreatures;
    }

    public Team GetCreatureTeam(string creatureId)
    {
        foreach (Team team in creatures.Keys)
        {
            if (creatures[team].Any(x => x.creature.ID == creatureId))
            {
                return team;
            }
        }
        return Team.DEAD;
    }

    public void RotateTokenToward(CharacterToken token, Vector3Int to)
    {
        Vector3Int from = characters.First(x => x.Value == token).Key;

        Vector3 flatFrom = GetPositionForHexFromCoordinate(new Vector2Int(from.x, from.z));
        Vector3 flatTo = GetPositionForHexFromCoordinate(new Vector2Int(to.x, to.z));

        Vector3 direction = (flatTo - flatFrom).normalized;

        token.SetLookRotation(direction);
    }

    public static IEnumerable<CharacterToken> GetTokensInRing(Vector3Int center, int minDist, int maxDist)
    {
        foreach (var kv in characters)
        {
            int d = CubeDistance(kv.Key, center);
            if (d > minDist && d <= maxDist)
                yield return kv.Value;
        }
    }

    public static IEnumerable<HexRenderer> HighlightHexesInRing(Vector3Int center, int minDist, int maxDist)
    {
        List<HexRenderer> result = new();
        foreach (var renderer in highlightedRenderers)
        {
            renderer.HideHighlight();
        }

        foreach (var kv in grid)
        {
            int d = CubeDistance(kv.Key, center);
            if (d > minDist && d <= maxDist)
            {
                HexRenderer hexRenderer = kv.Value.GetComponent<HexRenderer>();
                hexRenderer.ShowTargetHighlight();
                result.Add(hexRenderer);
            }
        }
        highlightedRenderers = result;
        return result;
    }

    public static IEnumerable<HexRenderer> HighlightCharactersInRing(Vector3Int center, int minDist, int maxDist)
    {
        List<HexRenderer> result = new();
        foreach (var renderer in highlightedRenderers)
        {
            renderer.HideHighlight();
        }

        foreach (var kv in grid)
        {
            int d = CubeDistance(kv.Key, center);
            if (d > minDist && d <= maxDist)
            {
                HexRenderer hexRenderer = kv.Value.GetComponent<HexRenderer>();
                hexRenderer.ShowTargetHighlight();
                result.Add(hexRenderer);
            }
        }
        highlightedRenderers = result;
        return result;
    }
}
