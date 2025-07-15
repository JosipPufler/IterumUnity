using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.Utils;
using Newtonsoft.Json;

namespace Assets.DTOs
{
    public class ActionDto
    {
        public ActionDto(){}

        public ActionDto(CustomBaseAction customBaseAction, string id = null)
        {
            Id = id;
            Name = customBaseAction.Name;
            Description = customBaseAction.Description;
            ApCost = customBaseAction.ApCost;
            MpCost = customBaseAction.MpCost;
            Data = JsonConvert.SerializeObject(customBaseAction, JsonSerializerSettingsProvider.GetSettings()); ;
        }

        public CustomBaseAction MapToCustomAction() {
            CustomBaseAction customBaseAction = JsonConvert.DeserializeObject<CustomBaseAction>(Data, JsonSerializerSettingsProvider.GetSettings());
            customBaseAction.Id = Id;
            return customBaseAction;
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ApCost { get; set; }
        public int MpCost { get; set; }
        public string Data { get; set; }
    }
}
