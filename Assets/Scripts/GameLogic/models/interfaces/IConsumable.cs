using Assets.Scripts.GameLogic.models.actions;
using Iterum.models.actions;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IConsumable : IItem
    {
        IAction ConsumeAction { get; }
        ActionResult Consume(List<IItem> source, ActionInfo actionInfo)
        {
            if (source.Contains(this))
            {
                ActionResult actionResult = ((IAction)ConsumeAction).ExecuteAction(actionInfo);
                source.Remove(this);
                return actionResult;
            }
            return null;
        }
    }
}
