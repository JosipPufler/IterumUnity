using System.Reflection;
using System;
using Iterum.models.interfaces;

namespace Assets.Scripts.Utils
{
    public static class StaticUtils
    {
        private static readonly string DisplayName = "DisplayName";
        private static readonly string Name = "Name";

        public static string GetDisplayName(Type type)
        {
            return GetProperty(type, DisplayName);
        }

        public static string GetName(Type type)
        {
            return GetProperty(type, Name);
        }

        private static string GetProperty(Type type, string propName)
        {
            if (!typeof(BaseCreature).IsAssignableFrom(type))
                throw new ArgumentException($"Type '{type.FullName}' must inherit from BaseCreature");

            var prop = type.GetProperty(propName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
                return (string)prop.GetValue(null);

            throw new ArgumentException($"No static field or property '{propName}' found on type '{type.FullName}'");
        }
    }
}
