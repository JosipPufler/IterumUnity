using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System.Collections.Generic;
using System.Linq;

namespace Iterum.models
{
    public class ArmorSet
    {
        public ArmorSet(ICreature creature)
        {
            this.creature = creature;
            IDictionary<ArmorSlot, int> armorSlots = creature.GetArmorSlots();
            armorSlots.Keys.ToList().ForEach(key =>
            {
                bool result = armorSlots.TryGetValue(key, out int number);
                if (result && number >= 0)
                    armors.Add(key, new IArmor[number]);
            });
        }

        private readonly IDictionary<ArmorSlot, IArmor[]> armors = new Dictionary<ArmorSlot, IArmor[]>();
        private readonly ICreature creature;

        public void RecalculateArmorSlots()
        {
            IDictionary<ArmorSlot, int> armorSlots = creature.GetArmorSlots();
            armorSlots.Keys.ToList().ForEach(key =>
            {
                bool result = armorSlots.TryGetValue(key, out int number);
                if (result && number >= 0) 
                {
                    armors[key] = new IArmor[number];
                    if (armors.TryGetValue(key, out IArmor[] oldArmor))
                    {
                        oldArmor = oldArmor.Where(x => x != null).ToArray();
                        int length = 0;
                        foreach (IArmor armor in oldArmor)
                        {
                            if (armors[key].Length >= length + 1)
                            {
                                creature.Inventory[armor] += 1;
                            }
                            else
                            {
                                armors[key][length] = armor;
                            }
                        }
                    }
                }
            });
        }

        public IDictionary<ArmorSlot, IArmor[]> GetArmorSet() {
            return armors;
        }

        public List<IArmor> GetArmor() {
            List<IArmor> listOfArmor = new();
            armors.Keys.ToList().ForEach(key =>
            {
                if (armors.TryGetValue(key, out IArmor[] listOfArmorInSlot) && listOfArmorInSlot.Length > 0)
                {
                    listOfArmor.AddRange(listOfArmorInSlot.Where(x => x != null).ToList());
                }
            });
            return listOfArmor;
        }

        public bool DonArmor(ICreature creature, IArmor armor, IList<IItem> source) {
            if (!source.Contains(armor))
            {
                return false;
            }
            if (armor.CanEquip(creature) && armors.ContainsKey(armor.ArmorSlot))
            {
                if (armors.TryGetValue(armor.ArmorSlot, out IArmor[] armorList))
                {
                    for (int i = 0; i < armorList.Length; i++)
                    {
                        if (armorList[i] == null)
                        {
                            armorList[i] = armor;
                            source.Remove(armor);
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public bool DoffArmor(IDictionary<IItem, int> targetInventory, IArmor armor)
        {
            if (armors.ContainsKey(armor.ArmorSlot))
            {
                bool result = armors.TryGetValue(armor.ArmorSlot, out IArmor[] armorList);
                if (result)
                {
                    for (int i = 0; i < armorList.Length; i++)
                    {
                        if (armorList[i] == armor)
                        {
                            armorList[i] = null;
                            if (targetInventory.ContainsKey(armor))
                            {
                                targetInventory[armor] += 1;
                            }
                            else
                            {
                                targetInventory[armor] = 1;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public IDictionary<DamageType, double> GetArmorSetResistances() {
            return DamageUtils.CalculateEfectiveDamage(GetArmor());
        }

        public int GetEvasionRatingModifier() {
            return GetArmor().Sum(x => x.EvasionRatingModifier);
        }

        public IList<IAction> GetActions()
        {
            List<IAction> actions = new List<IAction>();
            foreach (IArmor armor in GetArmor())
            {
                actions.AddRange(armor.Actions);
            }
            return actions;
        }

        public double GetSetWeight() {
            return GetArmor().Sum(x => x.Weight);
        }

        public Dictionary<Attribute, int> GetAttributeModifiers() {
            List<IArmor> armors1 = GetArmor();
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
