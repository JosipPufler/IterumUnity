using Iterum.models.enums;

namespace Assets.Scripts.GameLogic.models.interfaces
{
    public class ArmorGroup : BaseArmor
    {
        public ArmorGroup(ArmorSlot armorSlot) { 
            ArmorSlot = armorSlot;
        }
    }
}
