using Iterum.models.enums;
using Assets.Scripts.Utils.converters;

namespace Assets.Scripts.Utils
{
    public class DictionaryKeyArmorSlotConverter : DictionaryKeyConverterBase<ArmorSlot, object>
    {
        protected override string KeyToString(ArmorSlot key) => key.Name;
        protected override ArmorSlot StringToKey(string key) => ArmorSlot.FromName(key);
    }

    public class DictionaryKeyArmorSlotConverterInt : DictionaryKeyConverterBase<ArmorSlot, int>
    {
        protected override string KeyToString(ArmorSlot key) => key.Name;
        protected override ArmorSlot StringToKey(string key) => ArmorSlot.FromName(key);
    }
}
