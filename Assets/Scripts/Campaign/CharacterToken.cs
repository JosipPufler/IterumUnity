using System.Collections.Generic;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Managers;
using Iterum.models.interfaces;
using UnityEngine;
using Time = UnityEngine.Time;

public class CharacterToken : MonoBehaviour
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

    [Header("Data")]
    [SerializeField] public ICreature creature;
    public string controllerName;
    public bool forceOutline;

    const string outlineColorProperty = "_OutlineColor";
    ToolTipTrigger toolTipTrigger;
    MeshRenderer outlineRenderer;
    MaterialPropertyBlock propBlock;

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

    private void Start()
    {
        if (moveSpeed == 0 || moveSpeed == 4) {
            moveSpeed = 300f;
        }

        deadTop.SetActive(false);
        outline.SetActive(false);
        outlineRenderer = outline.GetComponent<MeshRenderer>();

        propBlock = new MaterialPropertyBlock();
        outlineRenderer.GetPropertyBlock(propBlock);
        SetOutlineColor(outlineColor);
        outlineRenderer.SetPropertyBlock(propBlock);
    }

    private void OnMouseEnter()
    {
        if (outlineRenderer != null && propBlock != null)
        {
            ShowOutline();
        }
    }

    private void OnMouseExit()
    {
        HideOutline();
    }

    void Update()
    {
        if (forceOutline)
        {
            ShowOutline();
        }
        else 
        {
            HideOutline();
        }

        if (!hasAction && actionQueue.Count > 0)
        {
            currentTokenAction = actionQueue.Dequeue();
            if (currentTokenAction.Type == TokenActionType.ROTATE && actionQueue.TryPeek(out TokenAction result) && result.Type == TokenActionType.MOVE && creature.CurrentAp == 0 && creature.MovementPoints == 0)
            {
                return;
            }
            if (currentTokenAction.Type == TokenActionType.MOVE && creature.MovementPoints == 0 && !creature.CreateMovementPoint())
            {
                return;
            }
            else if (currentTokenAction.Type == TokenActionType.MOVE)
            {
                creature.MovementPoints--;
            }
            hasAction = true;
        }

        if (!hasAction) return;

        switch (currentTokenAction.Type)
        {
            case TokenActionType.ROTATE:
                DoRotate();
                break;

            case TokenActionType.MOVE:
                DoMove();
                break;
        }
    }

    void DoRotate()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            currentTokenAction.TargetRot,
            rotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, currentTokenAction.TargetRot) < 0.1f)
        {
            transform.rotation = currentTokenAction.TargetRot;
            hasAction = false;
        }
    }

    void DoMove()
    {
        Vector3 current = transform.position;
        Vector3 target = currentTokenAction.TargetPos;

        if (current.y < target.y)
        {
            if (Mathf.Abs(current.y - target.y) > 0.001f)
            {
                Vector3 verticalTarget = new(current.x, target.y, current.z);
                transform.position = Vector3.MoveTowards(current, verticalTarget, moveSpeed * Time.deltaTime);
                return;
            }

            transform.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);

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
                return;
            }

            transform.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);

            if ((transform.position - target).sqrMagnitude < 0.0001f)
            {
                transform.position = target;
                hasAction = false;
            }
        }
    }

    public void SetLookRotation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        actionQueue.Enqueue(new TokenAction
        {
            Type = TokenActionType.ROTATE,
            TargetRot = Quaternion.LookRotation(direction, Vector3.up)
        });
    }

    public void MoveToken(Vector3 location) {
        actionQueue.Enqueue(new TokenAction
        {
            Type = TokenActionType.MOVE,
            TargetPos = location
        });
    }

    public void SetCreature(ICreature creature)
    {
        if (toolTipTrigger == null)
        {
            toolTipTrigger = gameObject.AddComponent<ToolTipTrigger>();
        }
        this.creature = creature;
        toolTipTrigger.tooltipText = creature.GetToolTipText();
        if (TextureMemorizer.textures.TryGetValue(creature.ImagePath, out Texture texture))
        {
            SetTexture(texture);
            return;
        }

        Texture2D texture2d = Resources.Load<Texture2D>(creature.ImagePath);

        if (texture2d == null)
        {
            AssetManager.Instance.GetImage(creature.ImagePath, SetTexture, error => Debug.Log(error));
        }
        else
        {
            SetTexture(texture2d);
        }
    }

    private void SetTexture(Texture texture) {
        Transform top = gameObject.transform.Find("top");
        if (top == null)
        {
            Debug.LogError("Child 'top' not found!");
            return;
        }

        top.GetComponent<Renderer>().material.mainTexture = texture;
        TextureMemorizer.textures[creature.ImagePath] = texture;
        gameObject.SetActive(true);
    }

    public void Die() 
    {
        creature.IsDead = true;
        body.GetComponent<MeshRenderer>().material = deadBodyMaterial;
        deadTop.SetActive(true);
    }

    public void Undie()
    {
        creature.IsDead = false;
        deadTop.SetActive(false);
        body.GetComponent<MeshRenderer>().material = normalBodyMaterial;
    }

    public void ShowOutline() {
        outline.SetActive(true);
    }

    public void HideOutline() { 
        outline.SetActive(false);
    }

    public void SetOutlineColor(Color color) {
        outlineColor = color;
        propBlock.SetColor(outlineColorProperty, outlineColor);
        outlineRenderer.SetPropertyBlock(propBlock);
    }
}
