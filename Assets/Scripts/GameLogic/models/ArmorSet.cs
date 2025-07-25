using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.Utils;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

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
                    armors.Add(key, new BaseArmor[number]);
            });
        }

        public ArmorSet(){}

        [JsonConverter(typeof(DictionaryKeyArmorSlotConverter))]
        private readonly IDictionary<ArmorSlot, BaseArmor[]> armors = new Dictionary<ArmorSlot, BaseArmor[]>();
        [JsonIgnore]
        private BaseCreature creature;

        public void SetCreature(BaseCreature creature) {
            this.creature = creature;
            armors.Clear();

            foreach (var kvp in creature.GetArmorSlots())
            {
                if (kvp.Value >= 0)
                {
                    armors[kvp.Key] = new BaseArmor[kvp.Value];
                }
            }
        }

        public void RecalculateArmorSlots()
        {
            IDictionary<ArmorSlot, int> armorSlots = creature.GetArmorSlots();
            armorSlots.Keys.ToList().ForEach(key =>
            {
                bool result = armorSlots.TryGetValue(key, out int number);
                if (result && number >= 0) 
                {
                    armors[key] = new BaseArmor[number];
                    if (armors.TryGetValue(key, out BaseArmor[] oldArmor))
                    {
                        oldArmor = oldArmor.Where(x => x != null).ToArray();
                        int length = 0;
                        foreach (BaseArmor armor in oldArmor)
                        {
                            if (armors[key].Length >= length + 1)
                            {
                                creature.Inventory.Add(armor);
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

        public IDictionary<ArmorSlot, BaseArmor[]> GetArmorSet() {
            return armors;
        }

        public List<BaseArmor> GetArmor() {
            List<BaseArmor> listOfArmor = new();
            armors.Keys.ToList().ForEach(key =>
            {
                if (armors.TryGetValue(key, out BaseArmor[] listOfArmorInSlot) && listOfArmorInSlot.Length > 0)
                {
                    listOfArmor.AddRange(listOfArmorInSlot.Where(x => x != null).ToList());
                }
            });
            return listOfArmor;
        }

        public bool DonArmor(BaseCreature creature, BaseArmor armor, IList<IItem> source) {
            if (!source.Contains(armor))
            {
                return false;
            }
            if (armor.CanEquip(creature) && armors.ContainsKey(armor.ArmorSlot))
            {
                if (armors.TryGetValue(armor.ArmorSlot, out BaseArmor[] armorList))
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


        public bool DoffArmor(List<BaseItem> targetInventory, BaseArmor armor)
        {
            if (armors.ContainsKey(armor.ArmorSlot))
            {
                bool result = armors.TryGetValue(armor.ArmorSlot, out BaseArmor[] armorList);
                if (result)
                {
                    for (int i = 0; i < armorList.Length; i++)
                    {
                        if (armorList[i] == armor)
                        {
                            armorList[i] = null;
                            targetInventory.Add(armor);
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
            List<IAction> actions = new();
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
