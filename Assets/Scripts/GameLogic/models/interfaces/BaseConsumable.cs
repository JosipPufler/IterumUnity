using Iterum.models;
using Iterum.models.actions;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Assets.Scripts.GameLogic.models.interfaces
{
    public class BaseConsumable : BaseItem
    {
        public BaseConsumable()
        {
            Initialize();
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            Initialize();
        }

        public virtual void Initialize(){}

        [JsonIgnore]
        public virtual Consume ConsumeAction { get; set; }
        public virtual ActionResult Consume(List<BaseItem> source, ActionInfo actionInfo)
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
