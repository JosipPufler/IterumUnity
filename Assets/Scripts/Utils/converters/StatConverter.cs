using System;
using Iterum.models.enums;
using Newtonsoft.Json;

namespace Assets.Scripts.Utils.converters
{
    public class StatConverter : JsonConverter<Stat>
    {
        public override void WriteJson(JsonWriter writer, Stat value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override Stat ReadJson(JsonReader reader, Type objectType, Stat existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string skillName = reader.Value?.ToString();
            return skillName != null ? Stat.FromName(skillName) : null;
        }
    }
}
