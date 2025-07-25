using System;
using System.Collections.Generic;
using System.Linq;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Attribute = Iterum.models.enums.Attribute;

namespace Assets.Scripts.GameLogic.models.creatures
{
    public abstract class BaseClass : IClass
    {
        public BaseClass(){}

        public BaseClass(BaseCreature creature)
        {
            Creature = creature;
            if (ClassActions.TryGetValue(1, out List<IAction> actions))
            {
                Actions = actions;
            }
            AttributesModifiers[Attribute.MaxHp] = (int)HealthDie + Creature.ModifierManager.GetAttribute(Attribute.Endurance, false);
        }

        public BaseCreature Creature { get; set; }

        public bool InitCreature(BaseCreature creature) {
            Creature = creature;
            if (ClassActions.TryGetValue(1, out List<IAction> actions))
            {
                Actions.Concat(actions);
            }
            AttributesModifiers[Attribute.MaxHp] = (int)HealthDie + Creature.ModifierManager.GetAttribute(Attribute.Endurance, false);
            return true;
        }

        public virtual string ClassName { get; }

        public virtual Dictionary<int, List<IAction>> ClassActions { get; } = new();

        public int Level { get; set; } = 1;

        public virtual Dice HealthDie { get; set; }

        public IList<IAction> Actions { get; } = new List<IAction>();

        public Dictionary<Attribute, int> AttributesModifiers {  get; } = new Dictionary<Attribute, int>();

        public Dictionary<Attribute, double> AttributesMultipiers { get; } = new Dictionary<Attribute, double>();

        public Dictionary<DamageCategory, double> DamageCategoryResistances { get; } = new Dictionary<DamageCategory, double>();

        public Dictionary<DamageType, double> Resistances { get; } = new Dictionary<DamageType, double>();

        public Dictionary<DamageCategory, double> CategoryResistances { get; } = new Dictionary<DamageCategory, double>();

        public string Description { get; set; }

        public abstract bool CanJoin(BaseCreature creature);

        public bool LevelUp()
        {
            Level += 1;
            if (ClassActions.ContainsKey(Level))
            {
                Actions.Concat(ClassActions[Level]);
            }

            if (AttributesModifiers.ContainsKey(Attribute.MaxHp))
            {
                AttributesModifiers[Attribute.MaxHp] += (int)Math.Ceiling((int)HealthDie / 2.0) + Creature.ModifierManager.GetAttribute(Attribute.Endurance, false);
            }
            else
            {
                AttributesModifiers[Attribute.MaxHp] = (int)Math.Ceiling((int)HealthDie / 2.0) + Creature.ModifierManager.GetAttribute(Attribute.Endurance, false);
            }
            return true;
        }
    }
}
