using Iterum.models.interfaces;
using System;
using System.Text;

namespace Assets.Scripts.GameLogic.models.interfaces
{
    public class BaseItem : IItem
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        public virtual double Weight { get; set; }

        public virtual bool Stackable { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string GetTooltipText()
        {
            StringBuilder sb = new();
            sb.AppendLine($"<size=200%><b>{Name}</b></size>");
            sb.AppendLine($"<size=150%>{Description}</size>");
            return sb.ToString();
        }
    }
}
