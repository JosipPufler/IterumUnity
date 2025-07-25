using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Iterum.models
{
    public class ActionResult
    {
        public ActionResult(BaseCreature source)
        {
            Source = source;
        }

        public bool Success { get; set; } = true;
        public BaseCreature Source { get; set; }
        public Dictionary<BaseCreature, int> AmountHealed { get; set; } = new Dictionary<BaseCreature, int>();
        public Dictionary<BaseCreature, IEnumerable<DamageResult>> AmountDamaged { get; set; } = new Dictionary<BaseCreature, IEnumerable<DamageResult>>();
        public Dictionary<BaseCreature, HashSet<StatusEffect>> StatusEffectsApplied { get; set; } = new Dictionary<BaseCreature, HashSet<StatusEffect>>();
        public Dictionary<BaseCreature, Dictionary<Attribute, int>> AttributesModified { get; set; } = new Dictionary<BaseCreature, Dictionary<Attribute, int>>();
        public List<string> ActionMessages = new();
    }
}
