using Assets.Scripts;
using Iterum.models.interfaces;
using UnityEngine;

public class ActionEntryData : MonoBehaviour
{
    public IAction action;

    public void SetAction(IAction action) { 
        this.action = action;
        UIToolTipTrigger toolTipTrigger = gameObject.GetComponent<UIToolTipTrigger>();
        if (toolTipTrigger == null) { 
            toolTipTrigger = gameObject.AddComponent<UIToolTipTrigger>();
        }
        toolTipTrigger.tooltipText = action.GetLongDescription();
    }
}
