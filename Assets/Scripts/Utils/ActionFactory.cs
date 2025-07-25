using Assets.Scripts.GameLogic.models.creatures;
using Iterum.models.actions;
using Iterum.models.creatures;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
    public static class ActionFactory
    {
        private static readonly HashSet<Type> creatures = new() { typeof(BasicMeleeWeaponAttack), typeof(AlphaWolf) };

        public static readonly Dictionary<string, Func<BaseCreature>> builtIns = new();

        static ActionFactory()
        {
            FillBuiltInActions(creatures);
        }

        private static void FillBuiltInActions(HashSet<Type> types)
        {
            foreach (Type type in types)
            {
                if (typeof(BaseCreature).IsAssignableFrom(type))
                {
                    builtIns.Add(type.Name, () => (BaseCreature)Activator.CreateInstance(type));
                }
            }
        }

        public static bool TryCreate(string id, out BaseCreature creature)
        {
            if (builtIns.TryGetValue(id, out var ctor))
            {
                creature = ctor();
                return true;
            }
            creature = null;
            return false;
        }
    }
}
