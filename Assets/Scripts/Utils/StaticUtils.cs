using System.Reflection;
using System;
using Iterum.models.interfaces;

namespace Assets.Scripts.Utils
{
    public static class StaticUtils
    {
        private static readonly string DisplayName = "DisplayName";

        public static string GetDisplayName(Type type) {
            if (!typeof(ICreature).IsAssignableFrom(type))
                throw new ArgumentException($"Type '{type.FullName}' must inherit from BaseCreature");

            var prop = type.GetProperty(DisplayName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
                return (string)prop.GetValue(null);

            throw new ArgumentException($"No static field or property '{DisplayName}' found on type '{type.FullName}'");
        }
    }
}
