using Iterum.models.enums;

namespace Iterum.models.interfaces
{
    public interface IArmor : ITargetable, IEquipment, IResistable
    { 
        TargetType GetTargetType() 
        { 
            return TargetType.Armor;
        }

        ArmorSlot ArmorSlot { get; }

        int EvasionRatingModifier 
        { 
            get => 0; 
        }

        TargetType ITargetable.TargetType
        {
            get
            {
                return TargetType.Armor;
            }
        }
    }
}
