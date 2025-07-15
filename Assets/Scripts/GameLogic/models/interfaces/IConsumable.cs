using Iterum.models.actions;
using System.Collections.Generic;

namespace Iterum.models.interfaces
{
    public interface IConsumable : IItem
    {
        Consume ConsumeAction { get; }
        ActionResult Consume(IDictionary<IItem, int> source, ActionInfo actionInfo)
        {
            if (source.ContainsKey(this))
            {
                ActionResult actionResult = ((IAction)ConsumeAction).ExecuteAction(actionInfo);
                source[this] -= 1;
                return actionResult;
            }
            return null;
        }
    }
}
