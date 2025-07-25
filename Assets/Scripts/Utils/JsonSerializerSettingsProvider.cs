using Assets.Scripts.Utils.converters;
using Newtonsoft.Json;
using System.Collections.Generic;

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
                }
            };
        }
    }
}
