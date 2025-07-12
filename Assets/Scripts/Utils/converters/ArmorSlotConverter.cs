using System;
using Newtonsoft.Json;

namespace Iterum.models.enums
{
    public class ArmorSlotConverter : JsonConverter<ArmorSlot>
    {
        public override void WriteJson(JsonWriter writer, ArmorSlot value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override ArmorSlot ReadJson(JsonReader reader, Type objectType, ArmorSlot existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var name = (string)reader.Value;
            return ArmorSlot.FromName(name);
        }
    }
}