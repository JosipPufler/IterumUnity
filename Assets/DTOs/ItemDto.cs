using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.items;
using Assets.Scripts.MainMenu;
using Assets.Scripts.Utils;
using Newtonsoft.Json;

namespace Assets.DTOs
{
    public class ItemDto
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public string Data  { get; set; }

        public BaseItem MapToBaseItem() {
            return Type switch
            {
                ItemType.Consumable => JsonConvert.DeserializeObject<BaseConsumable>(Data, JsonSerializerSettingsProvider.GetSettings()),
                ItemType.Weapon => JsonConvert.DeserializeObject<BaseWeapon>(Data, JsonSerializerSettingsProvider.GetSettings()),
                _ => null,
            };
        }
    }
}
