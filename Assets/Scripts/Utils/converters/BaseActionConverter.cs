using Assets.Scripts.GameLogic.models.actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Assets.Scripts.Utils.converters
{
    public class BaseActionConverter : JsonConverter<BaseAction>
    {
        public override BaseAction ReadJson(JsonReader reader, Type objectType, BaseAction existingValue, bool hasExistingValue, JsonSerializer serializer)
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

            var consumable = (BaseAction)jObject.ToObject(objectType, noLoopSerializer);

            consumable?.Initialize();

            return consumable;
        }

        public override void WriteJson(JsonWriter writer, BaseAction value, JsonSerializer serializer)
        {
            throw new NotImplementedException("This converter should only be used for reading.");
        }

        public override bool CanWrite => false;
    }
}
