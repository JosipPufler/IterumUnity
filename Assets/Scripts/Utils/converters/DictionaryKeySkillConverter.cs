using Iterum.models.enums;

namespace Assets.Scripts.Utils.converters
{
    public class DictionaryKeySkillConverter : DictionaryKeyConverterBase<Skill, int>
    {
        protected override string KeyToString(Skill key) => key.Name;
        protected override Skill StringToKey(string key) => Skill.FromName(key);
    }
}
