using Iterum.models.actions;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IConsumable : IItem
    {
        Consume ConsumeAction { get; }
        ActionResult Consume(IList<IItem> source, ActionInfo actionInfo)
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
