using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Assets.Scripts.Utils.converters
{
    public class BaseConsumableConverter : JsonConverter<BaseConsumable>
    {
        public override BaseConsumable ReadJson(JsonReader reader, Type objectType, BaseConsumable existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            var noLoopSerializer = new JsonSerializer
            {
                ContractResolver = serializer.ContractResolver,
                ObjectCreationHandling = serializer.ObjectCreationHandling,
                MissingMemberHandling = serializer.MissingMemberHandling,
                NullValueHandling = serializer.NullValueHandling,
                DefaultValueHandling = serializer.DefaultValueHandling
            };

            BaseConsumable consumable = (BaseConsumable)jObject.ToObject(objectType, noLoopSerializer);

            consumable?.Initialize();

            return consumable;
        }

        public override void WriteJson(JsonWriter writer, BaseConsumable value, JsonSerializer serializer)
        {
            throw new NotImplementedException("This converter should only be used for reading.");
        }

        public override bool CanWrite => false;
    }
}
