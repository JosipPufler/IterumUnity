using Iterum.models.interfaces;
using Mirror;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Utils.converters
{
    public static class CreatureSerializer
    {
        public static void Register()
        {
            Writer<BaseCreature>.write = WriteCreature;
            Reader<BaseCreature>.read = ReadCreature;
        }

        public static void WriteCreature(this NetworkWriter writer, BaseCreature value)
        {
            string json = JsonConvert.SerializeObject(value);
            Debug.Log("[WriteCreature] JSON: " + json);
            writer.WriteString(json);
        }

        public static BaseCreature ReadCreature(this NetworkReader reader)
        {
            string json = reader.ReadString();
            Debug.Log("[ReadCreature] JSON: " + json);
            var creature = JsonConvert.DeserializeObject<BaseCreature>(json);

            creature?.InitHelpers(default);

            return creature;
        }
    }
}
