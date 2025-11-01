using Assets.Scripts.GameLogic.models.actions;
using Iterum.models;
using Iterum.models.interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.Scripts.GameLogic.models.interfaces
{
    public class BaseConsumable : BaseItem
    {
        public BaseConsumable()
        {
            Initialize();
        }

        public BaseConsumable(BaseConsumable consumable)
        {
            ConsumeAction = consumable.ConsumeAction;
            Weight = consumable.Weight;
            Name = consumable.Name;
            Stackable = consumable.Stackable;
            Description = consumable.Description;
            Initialize();
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            Initialize();
        }

        public virtual void Initialize(){}
        
        public virtual BaseAction ConsumeAction { get; set; }
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

        public override string GetTooltipText()
        {
            StringBuilder stringBuilder = new(base.GetTooltipText());
            stringBuilder.AppendLine(ConsumeAction.ToString());
            return stringBuilder.ToString();
        }
    }
}
