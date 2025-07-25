using Assets.Scripts;
using Assets.Scripts.Campaign;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.converters;
using Iterum.models.interfaces;
using Mirror;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Time = UnityEngine.Time;

public class CharacterToken : NetworkBehaviour
{
    [Header("Token components")]
    public GameObject top;
    public GameObject deadTop;
    public GameObject body;
    public GameObject outline;

    [Header("Token materials")]
    public Material normalBodyMaterial;
    public Material deadTopMaterial;
    public Material deadBodyMaterial;
    public Color outlineColor = Color.white;

    [Header("Movement")]
    public float moveSpeed;
    public float rotationSpeed = 360f;

    [SyncVar]
    public GridCoordinate position;

    [Header("Data")]
    //[SyncVar(hook = nameof(OnCreatureChanged))] 
    public BaseCreature creature;
    [SyncVar(hook = nameof(OnCreatureJsonChanged))] public string creatureJson = "";

    [SyncVar] 
    public string controllerName;
    [SyncVar(hook = nameof(OnOutlineChanged))] 
    public bool forceOutline;
    [SyncVar(hook = nameof(OnOutlineChanged))]
    public bool userOutline;

    [SyncVar(hook = nameof(OnPositionChanged))]
    private Vector3 syncedPosition;

    [SyncVar(hook = nameof(OnRotationChanged))]
    private Quaternion syncedRotation;

    const string outlineColorProperty = "_OutlineColor";
    ToolTipTrigger toolTipTrigger;
    MeshRenderer outlineRenderer;
    MaterialPropertyBlock propBlock;

    [SyncVar(hook = nameof(OnDeadChanged))]
    public bool IsDead;

    enum TokenActionType { MOVE, ROTATE }

    struct TokenAction
    {
        public TokenActionType Type;
        public Quaternion TargetRot;
        public Vector3 TargetPos;
    }
    readonly Queue<TokenAction> actionQueue = new();
    TokenAction currentTokenAction;
    bool hasAction;

    bool initialized;

    [SyncVar(hook = nameof(OnTeamChanged))]
    public Team team;

    public Material playerMaterial, allyMaterial, enemyMaterial, neutralMaterial, deadMaterial;

    void OnTeamChanged(Team _, Team newTeam)
    {
        ApplyTeamStyle(newTeam);
    }

    void OnDeadChanged(bool _, bool isDead) {
        if (isDead)
        {
            DieVisually();
        }
        else
        {
            ApplyTeamStyle(team);
            deadTop.SetActive(false);
        }
    }

    public void ApplyTeamStyle(Team team)
    {
        Material mat = null;
        switch (team)
        {
            case Team.PLAYER:
                mat = playerMaterial;
                outlineColor = Color.blue;
                break;
            case Team.ALLY:
                mat = allyMaterial;
                outlineColor = Color.green;
                break;
            case Team.ENEMY:
                mat = enemyMaterial;
                outlineColor = Color.red;
                break;
            case Team.NEUTRAL:
                mat = neutralMaterial;
                outlineColor = Color.white;
                break;
            case Team.DEAD:
                mat = deadMaterial;
                outlineColor = Color.gray;
                break;
        }

        normalBodyMaterial = mat;

        var body = transform.Find("body");
        if (body && body.TryGetComponent<MeshRenderer>(out var renderer))
            renderer.material = mat;

        if (body && !body.TryGetComponent<Collider>(out _))
        {
            var col = body.gameObject.AddComponent<BoxCollider>();
            col.size = new Vector3(1, 2, 1);
        }

        transform.localScale = new Vector3(1.6f, 1f, 1.6f);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        CampaignGridLayout.Instance.RegisterToken(position, this);
        ApplyTeamStyle(team);
        TryInit();
    }

    public override void OnStartServer()
    {
        base.OnStartClient();

        CampaignGridLayout.Instance.RegisterToken(position, this);
        ApplyTeamStyle(team);
        InitVisuals();
    }

    void TryInit()
    {
        if (initialized || creature == null) return;
        InitVisuals();

        initialized = true;
    }

    void InitVisuals()
    {
        deadTop.SetActive(false);
        outline.SetActive(false);

        outlineRenderer = outline.GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();
        outlineRenderer.GetPropertyBlock(propBlock);
        SetOutlineColor(outlineColor);
        outlineRenderer.SetPropertyBlock(propBlock);

        if (!toolTipTrigger && isClient)
            toolTipTrigger = gameObject.AddComponent<ToolTipTrigger>();
        ApplyTeamStyle(team);

        if (isServer)
        {
            return;
        }
        TextureMemorizer.LoadTexture(creature.ImagePath, texture => top.GetComponent<MeshRenderer>().material.mainTexture = texture);
    }

    void Update()
    {
        if (creature == null)
        {
            return;
        }

        IsDead = creature.IsDead;

        if (isClient && toolTipTrigger != null && creature != null)
            toolTipTrigger.tooltipText = creature.GetToolTipText();

        if (!isServer) return;
    
        if (!hasAction && actionQueue.Count > 0)
        {
            currentTokenAction = actionQueue.Dequeue();
            hasAction = true;
        }
        if (!hasAction) return;

        switch (currentTokenAction.Type)
        {
            case TokenActionType.ROTATE: DriveRotate(); break;
            case TokenActionType.MOVE: DriveMove(); break;
        }
    }

    void DriveRotate()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            currentTokenAction.TargetRot,
            rotationSpeed * Time.deltaTime);

        syncedRotation = transform.rotation;
        if (Quaternion.Angle(transform.rotation, currentTokenAction.TargetRot) < 0.1f)
        {
            transform.rotation = currentTokenAction.TargetRot;
            hasAction = false;
        }
    }

    void DriveMove()
    {
        Vector3 current = transform.position;
        Vector3 target = currentTokenAction.TargetPos;
        
        if (current.y < target.y)
        {
            if (Mathf.Abs(current.y - target.y) > 0.001f)
            {
                Vector3 verticalTarget = new(current.x, target.y, current.z);
                transform.position = Vector3.MoveTowards(current, verticalTarget, moveSpeed * Time.deltaTime);

                syncedPosition = transform.position;
                return;
            }

            transform.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);

            syncedPosition = transform.position;
            if ((transform.position - target).sqrMagnitude < 0.0001f)
            {
                transform.position = target;
                hasAction = false;
            }
        }
        else
        {
            if (Mathf.Abs(current.x - target.x) > 0.001f || Mathf.Abs(current.z - target.z) > 0.001f)
            {
                Vector3 horizontalTarget = new(target.x, current.y, target.z);
                transform.position = Vector3.MoveTowards(current, horizontalTarget, moveSpeed * Time.deltaTime);

                syncedPosition = transform.position;
                return;
            }

            transform.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);

            syncedPosition = transform.position;
            if ((transform.position - target).sqrMagnitude < 0.0001f)
            {
                transform.position = target;
                hasAction = false;
            }
        }
    }

    void DieVisually() {
        deadTop.SetActive(true);
        var body = transform.Find("body");
        if (body && body.TryGetComponent<MeshRenderer>(out var renderer))
            renderer.material = deadBodyMaterial;
    }

    public void QueueRotate(Vector3 direction)
    {
        if (!isOwned || direction == Vector3.zero) return;
        CmdQueueRotate(direction);
    }

    [Command]
    void CmdQueueRotate(Vector3 direction)
    {
        actionQueue.Enqueue(new TokenAction { Type = TokenActionType.ROTATE, TargetRot = Quaternion.LookRotation(direction, Vector3.up) });
    }

    public void QueueMove(Vector3 worldPos)
    {
        if (!isOwned) return;
        CmdQueueMove(worldPos);
    }

    [Command]
    void CmdQueueMove(Vector3 pos)
    {
        if (creature.MovementPoints == 0 && !creature.CreateMovementPoint()) {
            return;
        }
        creature.MovementPoints--;
        actionQueue.Enqueue(new TokenAction { Type = TokenActionType.MOVE, TargetPos = pos });
        creatureJson = JsonConvert.SerializeObject(creature, JsonSerializerSettingsProvider.GetSettings());
    }

    void ShowOutline() => outline.SetActive(true);
    void HideOutline() => outline.SetActive(false);

    void SetOutlineColor(Color color)
    {
        outlineColor = color;
        propBlock.SetColor(outlineColorProperty, outlineColor);
        outlineRenderer.SetPropertyBlock(propBlock);
    }

    private void OnPositionChanged(Vector3 _, Vector3 newPos)
    {
        transform.position = newPos;
    }

    private void OnRotationChanged(Quaternion _, Quaternion newRot)
    {
        transform.rotation = newRot;
    }

    private void OnOutlineChanged(bool _, bool _2) {
        if (!userOutline && !forceOutline)
        {
            HideOutline();
        }
        else if (userOutline || forceOutline)
        {
            ShowOutline();
        }
    }

    void OnCreatureJsonChanged(string _, string newJson)
    {
        Debug.Log("[Client] Received creature JSON: " + newJson);

        if (!string.IsNullOrWhiteSpace(newJson))
        {
            if (ConverterUtils.TryParseCharacter(newJson, out DownableCreature character))
            {
                creature = character;
                creature?.InitHelpers(default);
            }
            else if (ConverterUtils.TryParseCreature(newJson, out BaseCreature baseCreature))
            {
                creature = baseCreature;
                creature?.InitHelpers(default);
            }
            else
            {
                Debug.Log("[Client] Could not parse JSON");
                return;
            }

            if (!initialized)
            {
                InitVisuals();
                initialized = true;
            }
        }
    }

    private void OnDestroy()
    {
        if (CampaignGridLayout.Instance == null) return;

        CampaignGridLayout.Instance.UnregisterToken(position, this);
    }
}