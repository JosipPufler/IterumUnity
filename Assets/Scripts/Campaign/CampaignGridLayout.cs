using Assets.Scripts;
using Assets.Scripts.Campaign;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.Map;
using Assets.Scripts.Network;
using Iterum.DTOs;
using Iterum.Scripts.UI;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CampaignGridLayout : HexGridLayout
{
    private static CampaignGridLayout _instance;
    public static CampaignGridLayout Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject); // Optional: Enforce singleton
    }

    private const string TokenTag = "Token";
    private const string HexTag = "Hex";
    public MapDto currentMap;
    public bool MapLoaded { get; set; } = false;

    public GeneralManager manager;
    //public CampaignActionManager actionManager;
    public GameObject hexPrefab;

    [Header("Token")]
    public GameObject tokenPrefab;
    public Material allyMaterial;
    public Material neutralMaterial;
    public Material enemyMaterial;
    public Material playerMaterial;
    public Material deadMaterial;

    private readonly Dictionary<Team, List<CharacterToken>> creatures = new();

    private static readonly Dictionary<GridCoordinate, CharacterToken> characters = new();
    private CharacterToken selectedToken;

    private readonly Dictionary<GridCoordinate, HexRenderer> selectedHexes = new();
    private bool movin = false;
    private int currentMoveNumber = 0;
    private GridCoordinate lastHex;

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
        if (!NetworkClient.localPlayer) return;

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
                                Debug.Log($"{pair.Key.x} {pair.Key.y} {pair.Key.z}");

                                if (CampaignActionManager.Instance.LookingForTargets && IsWithinRange(GetTokenCoordinates(CampaignActionManager.Instance.CurrentToken), pair.Key, CampaignActionManager.Instance.CurrentTargetData.MinDistance, CampaignActionManager.Instance.CurrentTargetData.MaxDistance)) {
                                    CampaignActionManager.Instance.SubmitHexTarget(new TargetDataSubmissionHex(CampaignActionManager.Instance.CurrentTargetData, pair.Key));
                                }
                                GridCoordinate aboveKey = new(pair.Key.x, pair.Key.y + 1, pair.Key.z);
                                if (!CampaignPlayer.LocalPlayer) return;

                                if (GameManager.Instance.SelectedCharacter != null)
                                {
                                    CampaignPlayer.LocalPlayer.TrySpawnCharacter(GameManager.Instance.SelectedCharacter, aboveKey, CampaignPlayer.LocalPlayer.isCampaignHost ? GameManager.Instance.Team : Team.PLAYER);
                                }
                                else if (GameManager.Instance.SelectedCreature != null)
                                {
                                    CampaignPlayer.LocalPlayer.TrySpawnCreature(GameManager.Instance.SelectedCreature.GetType().Name, aboveKey, CampaignPlayer.LocalPlayer.isCampaignHost ? GameManager.Instance.Team : Team.ALLY);
                                }
                                break;
                            }
                        }
                    }
                }

                if (hitObject.CompareTag(TokenTag))
                {
                    CharacterToken characterToken = hitObject.GetComponentInParent<CharacterToken>();
                    GridCoordinate tokenCoords = GetTokenCoordinates(characterToken);

                    if (characterToken.isOwned && IsTokenCurrentTurn(characterToken))
                    {
                        selectedToken = characterToken;
                        movin = true;
                        tokenCoords.y -= 1;
                        lastHex = tokenCoords;
                    }

                    if (CampaignActionManager.Instance.LookingForTargets && IsWithinRange(GetTokenCoordinates(manager.GetCurrentToken()), tokenCoords, CampaignActionManager.Instance.CurrentTargetData.MinDistance, CampaignActionManager.Instance.CurrentTargetData.MaxDistance))
                    {
                        CampaignActionManager.Instance.SubmitCreatureTarget(new TargetDataSubmissionCreature(CampaignActionManager.Instance.CurrentTargetData, characterToken.GetComponent<NetworkIdentity>().netId));
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
                                GridCoordinate currentKey = pair.Key;
                                if (movin)
                                {
                                    if (currentMoveNumber == selectedToken.creature.EffectiveMovementPoints || selectedHexes.ContainsKey(currentKey))
                                    {
                                        break;
                                    }
                                    if (!AreAdjacent(lastHex, currentKey))
                                    {
                                        Debug.Log($"Not close {lastHex}, {currentKey}");
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
            foreach (GridCoordinate hex in selectedHexes.Keys)
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
        return manager.GetCurrentToken() != null && manager.GetCurrentToken().creature == characterToken.creature;
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

    private void MoveToken(CharacterToken selectedToken, GridCoordinate aboveKey)
    {
        if (characters.TryGetValue(aboveKey, out CharacterToken _))
        {
            return;
        }
        GridCoordinate from = GetTokenCoordinates(selectedToken);
        characters.Remove(from);
        characters[aboveKey] = selectedToken;
        selectedToken.QueueMove(GetPositionForTokenInRealWorld(aboveKey, selectedToken.gameObject));
    }

    private static GridCoordinate GetTokenCoordinates(CharacterToken selectedToken)
    {
        return characters.First(x => x.Value == selectedToken).Key;
    }

    public void RegisterToken(GridCoordinate hexKey, CharacterToken token)
    {
        if (characters.ContainsKey(hexKey))
        {
            Debug.LogWarning($"Token already registered at {hexKey}");
            return;
        }

        characters[hexKey] = token;

        if (!creatures.TryGetValue(token.team, out var teamList))
        {
            teamList = new List<CharacterToken>();
            creatures[token.team] = teamList;
        }

        teamList.Add(token);
        //manager.UpdateInitiative(token);
    }

    public void UnregisterToken(GridCoordinate hexKey, CharacterToken token)
    {
        characters.Remove(hexKey);

        if (creatures.TryGetValue(token.team, out var list))
        {
            list.Remove(token);
            if (list.Count == 0)
                creatures.Remove(token.team);
        }
    }

    /*private void TryAddToken(Vector3Int aboveKey)
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

        characterToken.creature = GameManager.Instance.SelectedCreature;
        characterToken.normalBodyMaterial = currentMaterial;
        manager.UpdateInitiative(characterToken);

        characters[aboveKey] = characterToken;
    }*/

    public Vector3 GetPositionForTokenInRealWorld(GridCoordinate aboveKey, GameObject token)
    {
        float bodyHeight = token.transform.Find("outlineBody").GetComponent<MeshRenderer>().bounds.size.y;

        Vector3 vector3 = GetPositionForHexFromCoordinate(new Vector2Int(aboveKey.x, aboveKey.z));
        vector3.y = aboveKey.y * height * 2 + (bodyHeight / 2f) + 1;
        return vector3;
    }

    [Server]
    public void LoadMap(MapDto mapDto)
    {
        foreach (var hex in grid.Values)
            Destroy(hex);

        grid.Clear();

        isFlatTopped = mapDto.IsFlatTopped;
        gridSize.x = mapDto.MaxX;
        gridSize.y = mapDto.MaxY;

        foreach (var hex in mapDto.Hexes)
            TryAddHex(new GridCoordinate(hex.X, hex.Y, hex.Z));

        MapLoaded = true;
    }

    [Server]
    public override void TryAddHex(GridCoordinate key)
    {
        if (!grid.ContainsKey(key))
        {
            GameObject tile = Instantiate(hexPrefab);
            tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(key.x, key.z));
            tile.tag = "Hex";

            HexRenderer hexRenderer = tile.GetComponent<HexRenderer>();
            hexRenderer.positon = key;
            if (key.y == 0)
                hexRenderer.Initialize(hexMaterial, highlightMaterial, targetMaterial, 0, outerSize-5, height * 2 + 1000, isFlatTopped, -1000);
            else
                hexRenderer.Initialize(hexMaterial, highlightMaterial, targetMaterial, 0, outerSize-5, height * 2, isFlatTopped, key.y * height * 2);

            tile.transform.SetParent(transform, true);
            grid[key] = tile;

            if (NetworkServer.active)
            {
                Debug.Log($"Spawning tile {key.x}, {key.y}, {key.z}");
                NetworkServer.Spawn(tile);
            }
        }
    }

    [Client]
    public void RegisterHex(GridCoordinate coord, GameObject hex)
    {
        if (!grid.ContainsKey(coord))
            grid.Add(coord, hex);
    }

    [Client]
    public void UnregisterHex(GridCoordinate coord)
    {
        grid.Remove(coord);
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

    public void RotateTokenToward(CharacterToken token, GridCoordinate to)
    {
        GridCoordinate from = characters.First(x => x.Value == token).Key;

        Vector3 flatFrom = GetPositionForHexFromCoordinate(new Vector2Int(from.x, from.z));
        Vector3 flatTo = GetPositionForHexFromCoordinate(new Vector2Int(to.x, to.z));

        Vector3 direction = (flatTo - flatFrom).normalized;

        token.QueueRotate(direction);
    }

    public IEnumerable<CharacterToken> GetTokensInRing(GridCoordinate center, int minDist, int maxDist)
    {
        foreach (var kv in characters)
        {
            int distance = CubeDistance(kv.Key, center);
            if (distance >= minDist && distance <= maxDist)
                yield return kv.Value;
        }
    }

    public IEnumerable<HexRenderer> HighlightHexesInRing(GridCoordinate center, int minDist, int maxDist)
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

    public IEnumerable<HexRenderer> HighlightCharactersInRing(GridCoordinate center, int minDist, int maxDist)
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
