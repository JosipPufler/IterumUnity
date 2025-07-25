using Assets.Scripts.GameLogic.models.items;
using Iterum.models.interfaces;
using Newtonsoft.Json;

namespace Assets.Scripts.GameLogic.models.actions
{
    public class WeaponAction : BaseAction
    {
        [JsonIgnore]
        public BaseWeapon weapon;
    }
}
