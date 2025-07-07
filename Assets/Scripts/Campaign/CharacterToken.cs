using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Managers;
using Iterum.models.interfaces;
using UnityEngine;

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

    public ICreature creature;
    ToolTipTrigger toolTipTrigger;
    private MeshRenderer outlineRenderer;
    private MaterialPropertyBlock propBlock;

    private void Start()
    {
        deadTop.SetActive(false);
        outline.SetActive(false);
        outlineRenderer = outline.GetComponent<MeshRenderer>();

        propBlock = new MaterialPropertyBlock();
        outlineRenderer.GetPropertyBlock(propBlock);
        outlineRenderer.SetPropertyBlock(propBlock);
    }

    private void OnMouseEnter()
    {
        if (outlineRenderer != null && propBlock != null)
        {
            propBlock.SetColor("_OutlineColor", Color.white);
            outlineRenderer.SetPropertyBlock(propBlock);
            ShowOutline();
        }
    }

    private void OnMouseExit()
    {
        HideOutline();
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
        body.GetComponent<MeshRenderer>().material = deadBodyMaterial;
        deadTop.SetActive(true);
    }

    public void Undie()
    {
        deadTop.SetActive(false);
        body.GetComponent<MeshRenderer>().material = normalBodyMaterial;
    }

    public void ShowOutline() {
        outline.SetActive(true);
    }

    public void HideOutline() { 
        outline.SetActive(false);
    }
}
