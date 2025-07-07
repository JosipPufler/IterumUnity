using System.Xml.Linq;

namespace Iterum.models.enums
{
    public class Skill
    {
        private Skill(string name, Stat stat, Attribute attribute)
        {
            Name = name;
            Stat = stat;
            Attribute = attribute;
        }

        public string Name { get; set; }
        public Attribute Attribute { get; }
        public Stat Stat { get; set; }

        public static readonly Skill Perception = new Skill("Perception", Stat.Endurance, Attribute.Perception);
    }
}
