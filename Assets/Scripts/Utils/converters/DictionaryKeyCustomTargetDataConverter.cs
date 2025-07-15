using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils.converters
{
    public class DictionaryKeyCustomTargetDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<CustomTargetData, ActionPackage>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = (IDictionary<CustomTargetData, ActionPackage>)value;

            var noTypeSer = new JsonSerializer { TypeNameHandling = TypeNameHandling.None };

            writer.WriteStartObject();
            foreach (var kvp in dict)
            {
                string keyStr = JsonConvert.SerializeObject(kvp.Key, Formatting.None,
                                   new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
                writer.WritePropertyName(keyStr);

                noTypeSer.Serialize(writer, kvp.Value);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var noTypeSer = new JsonSerializer { TypeNameHandling = TypeNameHandling.None };

            var raw = noTypeSer.Deserialize<Dictionary<string, ActionPackage>>(reader);

            var result = new Dictionary<CustomTargetData, ActionPackage>();
            foreach (var kvp in raw)
            {
                var keyObj = JsonConvert.DeserializeObject<CustomTargetData>(kvp.Key,
                                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
                result[keyObj] = kvp.Value;
            }
            return result;
        }
    }
}
