using Iterum.models.interfaces;
using Newtonsoft.Json;

namespace Assets.Scripts.Utils.converters
{
    public static class ConverterUtils
    {
        public static bool TryParseCreature(string json, out BaseCreature creature) {
            try
            {
                creature = JsonConvert.DeserializeObject<BaseCreature>(json, JsonSerializerSettingsProvider.GetSettings());
                return creature != null;
            }
            catch (JsonException e)
            {
                creature = null;
                return false;
            }
        }

        public static bool TryParseCharacter(string json, out DownableCreature creature)
        {
            try
            {
                creature = JsonConvert.DeserializeObject<DownableCreature>(json, JsonSerializerSettingsProvider.GetSettings());
                return creature != null;
            }
            catch (JsonException e)
            {
                creature = null;
                return false;
            }
        }
    }
}
