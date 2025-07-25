using Assets.Scripts.Campaign;
using UnityEngine;

public class BodyOutlineListener : MonoBehaviour
{
    public CharacterToken characterToken;

    void OnMouseEnter()
    {
        Debug.Log("Enter");
        CampaignPlayer.LocalPlayer.CmdUpdateTokenHighlight(characterToken.netId, true);
    }

    void OnMouseExit()
    {
        Debug.Log("Exit");
        CampaignPlayer.LocalPlayer.CmdUpdateTokenHighlight(characterToken.netId, false);
    }
}
