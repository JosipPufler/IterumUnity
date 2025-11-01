using Assets.Scripts.Utils;
using Iterum.models.interfaces;
using Newtonsoft.Json;

namespace Assets.DTOs
{
    public class CharacterDto
    {
        public CharacterDto(){}

        public CharacterDto(BaseCreature creature) {
            if (creature.CharacterId != null)
            {
                Id = creature.CharacterId;
            }
            Name = creature.Name;
            Level = creature.ClassManager.GetLevel();
            IsPlayer = creature.IsPlayer;
            Data = JsonConvert.SerializeObject((object)creature, JsonSerializerSettingsProvider.GetSettings());
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsPlayer { get; set; }
        public string Data { get; set; }
        public long OwnerId { get; set; }
    }
}
