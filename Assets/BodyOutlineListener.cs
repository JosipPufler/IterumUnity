using Assets.Scripts.Campaign;
using Mirror;
using UnityEngine;

public class BodyOutlineListener : MonoBehaviour
{
    public CharacterToken characterToken;

    ToolTipTrigger3D toolTipTrigger;

    private void Awake()
    {
        var ni = GetComponentInParent<NetworkIdentity>();
        if (ni != null && ni.isClient)
        {
            enabled = true;
            return;
        }
    }

    private void Start()
    {
        if (!enabled) return;

        if (!toolTipTrigger)
            toolTipTrigger = gameObject.AddComponent<ToolTipTrigger3D>();
    }

    void OnMouseEnter()
    {
        if (!enabled) return;
        CampaignPlayer.LocalPlayer.CmdUpdateTokenHighlight(characterToken.netId, true);
    }

    void OnMouseExit()
    {
        if (!enabled) return;
        CampaignPlayer.LocalPlayer.CmdUpdateTokenHighlight(characterToken.netId, false);
    }

    private void Update()
    {
        if (!enabled) return;
        if (toolTipTrigger != null && characterToken.creature != null) { }
            toolTipTrigger.tooltipText = characterToken.creature.GetToolTipText();
    }
}
