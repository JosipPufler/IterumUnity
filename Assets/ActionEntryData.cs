using Assets.Scripts;
using Assets.Scripts.Campaign;
using Iterum.models.interfaces;
using UnityEngine;
using UnityEngine.UI;

public class ActionEntryData : MonoBehaviour
{
    public IAction action;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            Debug.Log(action.ID); 
            CampaignActionManager.Instance.CmdSetAction(action.ID);
        });
    }

    public void SetAction(IAction action) { 
        this.action = action;
        if (!gameObject.TryGetComponent<UIToolTipTrigger>(out var toolTipTrigger)) { 
            toolTipTrigger = gameObject.AddComponent<UIToolTipTrigger>();
        }
        toolTipTrigger.tooltipText = action.GetLongDescription();
    }
}
