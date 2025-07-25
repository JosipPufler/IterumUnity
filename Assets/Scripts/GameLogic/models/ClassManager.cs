using Assets.Scripts.GameLogic.models.creatures;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models
{
    internal sealed class ClassComparer : IEqualityComparer<BaseClass>
    {
        public bool Equals(BaseClass x, BaseClass y)
        {
            return x.GetType() == y.GetType();
        }

        public int GetHashCode([DisallowNull] BaseClass obj)
        {
            return obj.GetType().GetHashCode();
        }
    }

    public class ClassManager
    {
        [JsonIgnore]
        public BaseCreature creature;
        public HashSet<BaseClass> Classes { get; private set; } = new(new ClassComparer());

        public ClassManager(BaseCreature creature)
        {
            this.creature = creature ?? throw new ArgumentNullException(nameof(creature));
        }

        public ClassManager(){}

        public void SetCreature(BaseCreature creature) {
            this.creature = creature;
            foreach (var characterClass in Classes)
            {
                characterClass.Creature = creature;
            }
        }

        public bool LevelUpClass<T>() where T : IClass, new() {
            IClass characterClass = Classes.First(x => x.GetType() == typeof(T));
            return characterClass != null && characterClass.LevelUp();
        }

        public bool StartClass<T>() where T : BaseClass, new()
        {
            BaseClass characterClass = new T();
            return characterClass.CanJoin(creature) && Classes.Add(characterClass);
        }

        public bool StartClass(Type classType)
        {
            if (!typeof(BaseClass).IsAssignableFrom(classType))
            {
                Debug.LogError($"Type {classType.Name} does not inherit from BaseClass");
                return false;
            }

            if (classType.GetConstructor(Type.EmptyTypes) == null)
            {
                Debug.LogError($"Type {classType.Name} does not have a default constructor");
                return false;
            }

            BaseClass characterClass = (BaseClass)Activator.CreateInstance(classType);
            return characterClass.CanJoin(creature) && characterClass.InitCreature(creature) && Classes.Add(characterClass);
        }

        public IList<BaseClass> GetAllClasses() { 
            return Classes.ToList();
        }

        #nullable enable
        public BaseClass? GetClass<T>() where T : BaseClass
        {
            return Classes.FirstOrDefault(x => x.GetType() == typeof(T));
        }
        #nullable disable

        public IList<IAction> GetClassActions()
        {
            return Classes.SelectMany(x => x.Actions).ToList();
        }

        public int GetLevel() 
        { 
            return Classes.Sum(x => x.Level);
        }

        public IDictionary<Attribute, int> GetAttributeModifiers() 
        {
            return Classes
                .SelectMany(characterClass => characterClass.AttributesModifiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Sum(kvp => kvp.Value));
        }

        public IDictionary<Attribute, double> GetAttributeMultipliers()
        {   
            return Classes
                .SelectMany(characterClass => characterClass.AttributesMultipiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product * entity.Value));
        }

        public IDictionary<DamageType, double> GetEffectiveDamageResistances() 
        {
            return DamageUtils.CalculateEfectiveDamage(Classes);
        }

        public IDictionary<DamageCategory, double> GetEffectiveDamageCategoryResistances()
        {
            return Classes
                .SelectMany(characterClass => characterClass.DamageCategoryResistances)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product * (1 - entity.Value)));
        }
    }
}
