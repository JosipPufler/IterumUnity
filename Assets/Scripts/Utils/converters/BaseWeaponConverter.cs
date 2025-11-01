using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Assets.Scripts.Utils.converters
{
    public class BaseWeaponConverter : JsonConverter<BaseWeapon>
    {
        public override BaseWeapon ReadJson(JsonReader reader, Type objectType, BaseWeapon existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            var noLoopSerializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = serializer.ContractResolver,
                ObjectCreationHandling = serializer.ObjectCreationHandling,
                MissingMemberHandling = serializer.MissingMemberHandling,
                NullValueHandling = serializer.NullValueHandling,
                DefaultValueHandling = serializer.DefaultValueHandling
            };

            var weapon = (BaseWeapon)jObject.ToObject(objectType, noLoopSerializer);

            foreach (var a in weapon.Actions)
            {
                if (a is WeaponAction weaponAction) {
                    weaponAction.Weapon = weapon; 
                }
                a.Initialize();
            }

            return weapon;
        }

        public override void WriteJson(JsonWriter writer, BaseWeapon value, JsonSerializer serializer)
        {
            throw new NotImplementedException("This converter should only be used for reading.");
        }

        public override bool CanWrite => false;
    }
}
