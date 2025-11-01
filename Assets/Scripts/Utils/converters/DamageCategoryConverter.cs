using Iterum.models.enums;
using Newtonsoft.Json;
using System;

namespace Assets.Scripts.Utils.converters
{
    public class DamageCategoryConverter : JsonConverter<DamageCategory>
    {
        public override void WriteJson(JsonWriter writer, DamageCategory value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override DamageCategory ReadJson(JsonReader reader, Type objectType, DamageCategory existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var name = (string)reader.Value;
            return DamageCategory.FromName(name);
        }
    }

    public class DictionaryKeyDamageCategoryConverter : DictionaryKeyConverterBase<DamageCategory, double>
    {
        protected override string KeyToString(DamageCategory key) => key.Name;
        protected override DamageCategory StringToKey(string key) => DamageCategory.FromName(key);
    }
}
