using Assets.Scripts.Utils;
using Iterum.models.interfaces;
using Mirror.Examples.CharacterSelection;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace Assets.DTOs
{
    public class CharacterDto
    {
        public CharacterDto(string id, string name, int level, bool isPlayer, string data)
        {
            Id = id;
            Name = name;
            Level = level;
            IsPlayer = isPlayer;
            Data = data;
        }

        public ICreature MapToCreature() {
            return JsonConvert.DeserializeObject<ICreature>(Data, JsonSerializerSettingsProvider.GetSettings());
        }
        public string? Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsPlayer { get; set; }
        public string Data { get; set; }
    }
}
