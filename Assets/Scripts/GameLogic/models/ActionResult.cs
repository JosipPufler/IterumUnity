using Iterum.models.enums;
using Iterum.models.interfaces;
using System.Collections.Generic;

namespace Iterum.models
{
    public class ActionResult
    {
        public ActionResult(IGameEntity source)
        {
            Source = source;
        }

        public bool Success { get; set; } = true;
        public IGameEntity Source { get; set; }
        public Dictionary<IDamageable, int> AmountHealed { get; set; } = new Dictionary<IDamageable, int>();
        public Dictionary<IDamageable, IEnumerable<DamageResult>> AmountDamaged { get; set; } = new Dictionary<IDamageable, IEnumerable<DamageResult>>();
        public Dictionary<ICreature, HashSet<StatusEffect>> StatusEffectsApplied { get; set; } = new Dictionary<ICreature, HashSet<StatusEffect>>();
        public Dictionary<ICreature, Dictionary<Attribute, int>> AttributesModified { get; set; } = new Dictionary<ICreature, Dictionary<Attribute, int>>();
        public List<string> ActionMessages = new();
    }
}
