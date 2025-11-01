using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.Utils;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models
{
    public class ArmorSet
    {
        public ArmorSet(BaseCreature creature)
        {
            this.creature = creature;
            IDictionary<ArmorSlot, int> armorSlots = creature.GetArmorSlots();
            armorSlots.Keys.ToList().ForEach(key =>
            {
                bool result = armorSlots.TryGetValue(key, out int number);
                if (result && number >= 0)
                    armors.Add(key, new List<BaseArmor>());
            });
        }

        public ArmorSet(){}

        public Dictionary<ArmorSlot, List<BaseArmor>> armors = new();

        [JsonIgnore]
        public BaseCreature creature;

        public void CreateArmorSet<T>(Dictionary<ArmorSlot, int> slots)
            where T : ArmorGroup
        {   
            foreach (var kvp in slots)
            {
                ArmorSlot slot = kvp.Key;
                int numberOfPieces = kvp.Value;

                for (int i = 0; i < numberOfPieces; i++)
                {
                    T armor = (T)Activator.CreateInstance(typeof(T), slot);
                    AddArmor(armor);
                }
            }
        }

        public void RecalculateArmorSlots()
        {
            foreach (var kvp in creature.GetArmorSlots())
            {
                int slotCount = kvp.Value;
                if (slotCount < 0)
                    continue;

                var oldArmor = armors.TryGetValue(kvp.Key, out var existing)
                    ? existing.Where(a => a != null).ToList()
                    : new List<BaseArmor>();

                armors[kvp.Key] = new List<BaseArmor>(new BaseArmor[slotCount]);

                for (int i = 0; i < oldArmor.Count; i++)
                {
                    if (i < slotCount)
                    {
                        armors[kvp.Key][i] = oldArmor[i];
                    }
                    else
                    {
                        creature.Inventory.Add(oldArmor[i]);
                    }
                }
            }
        }

        public IDictionary<ArmorSlot, List<BaseArmor>> GetArmorSet() {
            return armors;
        }

        public List<BaseArmor> GetArmor()
        {
            return armors
                .SelectMany(kvp => kvp.Value)
                .Where(a => a != null)
                .ToList();
        }

        public bool DonArmor(BaseArmor armor, List<BaseItem> source) {
            if (source.Contains(armor) && AddArmor(armor)) { 
                source.Remove(armor);
                return true;
            }
            return false;
        }

        public bool AddArmor(BaseArmor armor)
        {
            if (!armor.CanEquip(creature) || !armors.TryGetValue(armor.ArmorSlot, out var armorList))
                return false;
            Dictionary<ArmorSlot, int> dictionary = creature.GetArmorSlots();
            if (dictionary.TryGetValue(armor.ArmorSlot, out int maxSlots) && (armorList.Count + 1) <= maxSlots)
            {
                armorList.Add(armor);
                return true;
            }
            return false;
        }


        public bool DoffArmor(List<BaseItem> targetInventory, BaseArmor armor)
        {
            if (!armor.CanEquip(creature) || !armors.TryGetValue(armor.ArmorSlot, out var armorList))
                return false;
            if (armorList.Contains(armor))
            {
                armorList.Remove(armor);
                targetInventory.Add(armor);
                return true;
            }
            return false;
        }

        public IDictionary<DamageType, double> GetArmorSetResistances() {
            return DamageUtils.CalculateEfectiveDamageResistances(GetArmor());
        }

        public int GetEvasionRatingModifier() {
            return GetArmor().Sum(x => x.EvasionRatingModifier);
        }

        public IList<interfaces.IAction> GetActions()
        {
            List<interfaces.IAction> actions = new();
            foreach (BaseArmor armor in GetArmor())
            {
                actions.AddRange(armor.Actions);
            }
            return actions;
        }

        public double GetSetWeight() {
            return GetArmor().Sum(x => x.Weight);
        }

        public Dictionary<Attribute, int> GetAttributeModifiers() {
            List<BaseArmor> armors1 = GetArmor();
            return GetArmor()
                .SelectMany(characterClass => characterClass.AttributeModifiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Sum(kvp => kvp.Value));
        }

        public Dictionary<Attribute, double> GetAttributeMultipliers()
        {
            return GetArmor()
                .SelectMany(characterClass => characterClass.AttributeMultipliers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product * entity.Value));
        }
    }
}
