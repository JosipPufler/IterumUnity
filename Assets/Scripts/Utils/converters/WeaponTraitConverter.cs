using Iterum.models.enums;
using Newtonsoft.Json;
using System;

namespace Assets.Scripts.Utils.converters
{
    public class WeaponTraitConverter : JsonConverter<WeaponTrait>
    {
        public override void WriteJson(JsonWriter writer, WeaponTrait value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override WeaponTrait ReadJson(JsonReader reader, Type objectType, WeaponTrait existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string name = (string)reader.Value;
            
            return WeaponTrait.ForName(name);
        }
    }
}
