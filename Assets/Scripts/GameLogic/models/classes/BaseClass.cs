using Assets.Scripts.GameLogic.models.actions;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Attribute = Iterum.models.enums.Attribute;

namespace Assets.Scripts.GameLogic.models.creatures
{
    public abstract class BaseClass : IClass
    {
        public BaseClass(){}

        [JsonIgnore]
        public BaseCreature Creature { get; set; }

        public virtual bool InitCreature(BaseCreature creature) {
            if (creature == null)
            {
                return false;
            }
            Creature = creature;
            AttributesModifiers[Attribute.MaxHp] = (int)HealthDie + Creature.ModifierManager.GetAttribute(Attribute.Endurance, false);
            return true;
        }

        public abstract string ClassName { get; }
        public abstract string Description { get; set; }
        public abstract Dice HealthDie { get; set; }

        [JsonProperty]
        public virtual Dictionary<int, List<IAction>> ClassActions { get; protected set; } = new();

        public int Level { get; set; } = 1;

        [JsonProperty]
        public virtual Dictionary<Attribute, int> AttributesModifiers { get; protected set; } = new Dictionary<Attribute, int>();
        [JsonProperty]
        public virtual Dictionary<Attribute, double> AttributesMultipiers { get; protected set; } = new Dictionary<Attribute, double>();
        [JsonProperty]
        public virtual Dictionary<DamageCategory, double> DamageCategoryResistances { get; protected set; } = new Dictionary<DamageCategory, double>();
        [JsonProperty]
        public virtual Dictionary<DamageType, double> Resistances { get; protected set; } = new Dictionary<DamageType, double>();
        [JsonProperty]
        public virtual Dictionary<DamageCategory, double> CategoryResistances { get; protected set; } = new Dictionary<DamageCategory, double>();


        public virtual bool CanJoin(BaseCreature creature) {
            return true;
        }

        public virtual bool LevelUp()
        {
            Level += 1;

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

        public List<IAction> GetAvailableActions() {
            return ClassActions.Where(x => x.Key <= Level).SelectMany(x => x.Value).ToList();
        }
    }
}
