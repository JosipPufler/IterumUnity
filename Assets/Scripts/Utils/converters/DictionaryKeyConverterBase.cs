using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Assets.Scripts.Utils.converters
{
    public abstract class DictionaryKeyConverterBase<TKey, TValue> : JsonConverter
    {
        protected abstract string KeyToString(TKey key);
        protected abstract TKey StringToKey(string key);

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<TKey, TValue>).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = (IDictionary<TKey, TValue>)value;

            writer.WriteStartObject();
            foreach (var kvp in dict)
            {
                writer.WritePropertyName(KeyToString(kvp.Key));
                serializer.Serialize(writer, kvp.Value);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = (IDictionary<TKey, TValue>)Activator.CreateInstance(objectType);
            var dict = serializer.Deserialize<Dictionary<string, TValue>>(reader);

            foreach (var kvp in dict)
            {
                var key = StringToKey(kvp.Key);
                result.Add(key, kvp.Value);
            }

            return result;
        }
    }
}
