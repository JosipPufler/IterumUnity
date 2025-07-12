using Assets.Scripts;
using Iterum.models.interfaces;
using UnityEngine;

public class ActionEntryData : MonoBehaviour
{
    public IAction action;

    public void SetAction(IAction action) { 
        this.action = action;
        if (!gameObject.TryGetComponent<UIToolTipTrigger>(out var toolTipTrigger)) { 
            toolTipTrigger = gameObject.AddComponent<UIToolTipTrigger>();
        }
        toolTipTrigger.tooltipText = action.GetLongDescription();
    }
}
