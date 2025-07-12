using System;
using Iterum.models.enums;
using Newtonsoft.Json;

namespace Assets.Scripts.Utils
{
    public class SkillConverter : JsonConverter<Skill>
    {
        public override void WriteJson(JsonWriter writer, Skill value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override Skill ReadJson(JsonReader reader, Type objectType, Skill existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string skillName = reader.Value?.ToString();
            return skillName != null ? Skill.FromName(skillName) : null;
        }
    }
}
