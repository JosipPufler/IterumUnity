using Iterum.models.enums;

namespace Assets.Scripts.Utils.converters
{
    public class DictionaryKeyStatConverter : DictionaryKeyConverterBase<Stat, object>
    {
        protected override string KeyToString(Stat key) => key.Name;
        protected override Stat StringToKey(string key) => Stat.FromName(key);
    }
}
