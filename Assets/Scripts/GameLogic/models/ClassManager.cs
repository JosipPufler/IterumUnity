using Assets.Scripts.GameLogic.models.creatures;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models
{
    internal sealed class ClassComparer : IEqualityComparer<IClass>
    {
        public bool Equals(IClass x, IClass y)
        {
            return x.GetType() == y.GetType();
        }

        public int GetHashCode([DisallowNull] IClass obj)
        {
            return obj.GetType().GetHashCode();
        }
    }

    public class ClassManager
    {
        public ICreature creature;
        public HashSet<IClass> classes { get; private set; } = new(new ClassComparer());

        public ClassManager(ICreature creature)
        {
            this.creature = creature ?? throw new ArgumentNullException(nameof(creature));
        }

        public ClassManager(){}

        public bool LevelUpClass<T>() where T : IClass, new() {
            IClass characterClass = classes.First(x => x.GetType() == typeof(T));
            return characterClass != null && characterClass.LevelUp();
        }

        public bool StartClass<T>() where T : BaseClass, new()
        {
            BaseClass characterClass = new T();
            return characterClass.CanJoin(creature) && classes.Add(characterClass);
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
            return characterClass.CanJoin(creature) && characterClass.InitCreature(creature) && classes.Add(characterClass);
        }

        public IList<IClass> GetAllClasses() { 
            return classes.ToList();
        }

        #nullable enable
        public IClass? GetClass<T>() where T : IClass
        {
            return classes.FirstOrDefault(x => x.GetType() == typeof(T));
        }
        #nullable disable

        public IList<IAction> GetClassActions()
        {
            return classes.SelectMany(x => x.Actions).ToList();
        }

        public int GetLevel() 
        { 
            return classes.Sum(x => x.Level);
        }

        public IDictionary<Attribute, int> GetAttributeModifiers() 
        {
            return classes
                .SelectMany(characterClass => characterClass.AttributesModifiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Sum(kvp => kvp.Value));
        }

        public IDictionary<Attribute, double> GetAttributeMultipliers()
        {   
            return classes
                .SelectMany(characterClass => characterClass.AttributesMultipiers)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product * entity.Value));
        }

        public IDictionary<DamageType, double> GetEffectiveDamageResistances() 
        {
            return DamageUtils.CalculateEfectiveDamage(classes);
        }

        public IDictionary<DamageCategory, double> GetEffectiveDamageCategoryResistances()
        {
            return classes
                .SelectMany(characterClass => characterClass.DamageCategoryResistances)
                .GroupBy(entity => entity.Key)
                .ToDictionary(group => group.Key, group => group.Aggregate(1.0, (product, entity) => product * (1 - entity.Value)));
        }
    }
}
