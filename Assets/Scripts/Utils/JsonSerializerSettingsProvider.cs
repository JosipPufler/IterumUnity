using Newtonsoft.Json;

namespace Assets.Scripts.Utils
{
    public static class JsonSerializerSettingsProvider
    {
        public static JsonSerializerSettings GetSettings() {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                Formatting = Formatting.None
            };
        }
    }
}
