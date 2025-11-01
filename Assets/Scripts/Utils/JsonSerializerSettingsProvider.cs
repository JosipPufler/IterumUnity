using Assets.Scripts.Utils.converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class JsonSerializerSettingsProvider
    {
        public static JsonSerializerSettings GetSettings() {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                Formatting = Formatting.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new TargetDataDictionaryConverter(),
                    new BaseWeaponConverter(),
                    new BaseConsumableConverter(),
                    new DamageCategoryConverter(),
                    new DamageTypeConverter(),
                    new DictionaryKeyArmorSlotConverter(),
                    new DictionaryKeyArmorSlotConverterInt(),
                    new DictionaryKeyArmorSlotConverterList(),
                    new DictionaryKeyDamageCategoryConverter(),
                    new DictionaryKeyDamageTypeConverter(),
                    new Newtonsoft.Json.Converters.StringEnumConverter()
                },
                Error = (sender, args) =>
                {
                    Debug.LogError($"[JsonError] Path: {args.ErrorContext.Path} | Message: {args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = false;
                }
            };
        }
    }

    public class ForceReadOnlyContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return objectType.GetMembers(flags)
                             .Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                             .ToList();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var propertyInfo = member as PropertyInfo;
                if (propertyInfo?.GetSetMethod(true) != null)
                {
                    prop.Writable = true;
                }
            }

            return prop;
        }
    }
}
