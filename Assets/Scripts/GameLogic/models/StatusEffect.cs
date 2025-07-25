using Iterum.models.enums;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models
{
    public class StatusEffect
    {
        private StatusEffect(string name, IDictionary<Attribute, int> attributeModifier, Action<BaseCreature> action)
        {
            Name = name;
            AttributeModifier = attributeModifier;
            Action = action;
        }

        public string Name { get; }
        public IDictionary<Attribute, int> AttributeModifier { get; }
        public Action<BaseCreature> Action { get; }
    }
}
