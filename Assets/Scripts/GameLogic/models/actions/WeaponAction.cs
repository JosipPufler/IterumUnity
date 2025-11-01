using Assets.Scripts.GameLogic.models.items;
using Newtonsoft.Json;

namespace Assets.Scripts.GameLogic.models.actions
{
    public class WeaponAction : BaseAction
    {
        public WeaponAction() {
            Initialize();
        }

        public WeaponAction(WeaponAction weaponAction) : base(weaponAction){
            Weapon = weaponAction.Weapon;
        }

        [JsonIgnore]
        public BaseWeapon Weapon { get; set; }
    }
}
