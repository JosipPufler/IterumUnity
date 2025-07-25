using Assets.Scripts.GameLogic.models.creatures;
using Iterum.models.creatures;
using Iterum.models.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public static class CreatureFactory
    {
        private static readonly HashSet<Type> creatures = new() { typeof(Wolf), typeof(AlphaWolf) };

        public static readonly Dictionary<string, Func<BaseCreature>> builtIns = new();

        static CreatureFactory()
        {
            FillBuiltInCreatures(creatures);
        }

        private static void FillBuiltInCreatures(HashSet<Type> types) {
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
