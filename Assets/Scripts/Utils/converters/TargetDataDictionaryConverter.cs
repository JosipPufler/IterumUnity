using Assets.Scripts.GameLogic.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils.converters
{
    public class TargetDataDictionaryConverter : JsonConverter<Dictionary<TargetData, int>>
    {
        public override void WriteJson(JsonWriter writer, Dictionary<TargetData, int> dict, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var kv in dict)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Data");
                serializer.Serialize(writer, kv.Key);
                writer.WritePropertyName("Count");
                writer.WriteValue(kv.Value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public override Dictionary<TargetData, int> ReadJson(JsonReader reader,
             Type objectType,
             Dictionary<TargetData, int> existingValue,
             bool hasExistingValue,
             JsonSerializer serializer)
        {
            var list = JArray.Load(reader);
            var result = new Dictionary<TargetData, int>();
            foreach (var entry in list)
            {
                var data = entry["Data"].ToObject<TargetData>(serializer);
                var count = entry["Count"].Value<int>();
                result[data] = count;
            }
            return result;
        }
    }
}
