using Iterum.models.enums;

namespace Iterum.models.interfaces
{
    public interface IMagicalAction : IAction
    {
        int MpCost { get; }
        SpellSchool SpellSchool { get; }
    }
}
