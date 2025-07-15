using Iterum.models.enums;
using Newtonsoft.Json;
using System;

namespace Assets.Scripts.Utils.converters
{
    public class DamageTypeConverter : JsonConverter<DamageType>
    {
        public override void WriteJson(JsonWriter writer, DamageType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override DamageType ReadJson(JsonReader reader, Type objectType, DamageType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var name = (string)reader.Value;
            return DamageType.FromName(name);
        }
    }
}
