using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.items;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models
{
    public class WeaponSet
    {
        public IList<BaseWeapon> Weapons { get; set; } = new List<BaseWeapon>();
        [JsonIgnore]
        public BaseCreature creature;

        public void SetCreature(BaseCreature baseCreature) { 
            creature = baseCreature;
            foreach (var weapon in Weapons)
            {
                weapon.Creature = baseCreature;
            }
        }

        public WeaponSet(BaseCreature creature)
        {
            this.creature = creature;
            foreach (var weapon in Weapons)
            {
                weapon.Creature = creature;
            }
        }

        public WeaponSet(){}

        public IDictionary<WeaponSlot, int> CalculateFreeWeaponSlots()
        {
            IDictionary<WeaponSlot, int> freeWeaponSlots = new Dictionary<WeaponSlot, int>(creature.GetWeaponSlots());
            foreach (IWeapon weapon in Weapons)
            {
                if (freeWeaponSlots.TryGetValue(weapon.WeaponSlotDetails.Slot, out int numberOfSlots))
                {
                    freeWeaponSlots[weapon.WeaponSlotDetails.Slot] = (numberOfSlots - weapon.WeaponSlotDetails.SlotsNeeded) < 0 ? throw new Exception("mismatch of weapon slots") : numberOfSlots - weapon.WeaponSlotDetails.SlotsNeeded;
                }
                else
                {
                    freeWeaponSlots[weapon.WeaponSlotDetails.Slot] = 0;
                }
            }
            return freeWeaponSlots;
        }

        public bool AddWeapon(BaseWeapon weapon)
        {
            if (weapon.CanEquip(creature) && CalculateFreeWeaponSlots().TryGetValue(weapon.WeaponSlotDetails.Slot, out int numberOfFreeSlots) && numberOfFreeSlots >= weapon.WeaponSlotDetails.SlotsNeeded)
            {
                Weapons.Add(weapon);
                weapon.Creature = creature;
                return true;
            }
            return false;
        }

        public bool EquipWeapon(BaseWeapon weapon, IList<IItem> source)
        {
            if (!source.Contains(weapon))
            {
                return false;
            }
            if (weapon.CanEquip(creature) && CalculateFreeWeaponSlots().TryGetValue(weapon.WeaponSlotDetails.Slot, out int numberOfFreeSlots) && numberOfFreeSlots >= weapon.WeaponSlotDetails.SlotsNeeded && source.Remove(weapon))
            {
                Weapons.Add(weapon);
                weapon.Creature = creature;
                return true;
            }
            return false;
        }

        public void UnequipWeapon(List<BaseItem> targetInventory, BaseWeapon weapon)
        {
            targetInventory.Add(weapon);
            Weapons.Remove(weapon);
        }

        public int GetEvasionRatingBonus()
        {
            return Weapons.Sum(x => x.EvasionRatingBonus);
        }

        public IList<IAction> GetActions()
        {
            List<IAction> actions = new();
            foreach (IWeapon weapon in Weapons)
            {
                actions.AddRange(weapon.WeaponActions);
            }
            return actions;
        }

        public double GetSetWeight()
        {
            return Weapons.Sum(x => x.Weight);
        }

        public Dictionary<Attribute, int> GetAttributeModifiers()
        {
            return Weapons
                .SelectMany(characterClass => characterClass.AttributeModifiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Sum(kvp => kvp.Value));
        }

        public Dictionary<Attribute, double> GetAttributeMultipliers()
        {
            return Weapons
                .SelectMany(characterClass => characterClass.AttributeMultipliers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product * entity.Value));
        }
    }
}
