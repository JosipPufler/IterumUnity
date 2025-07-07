namespace Iterum.models.interfaces
{
    public interface ISpellcaster
    {
        int CurrentMp { get; set; }
        int MaxMp { get; set; }
        int OriginalMaxMp { get; }
        void RegenMana(int mp) {
            if (mp > MaxMp)
            {
                CurrentMp = MaxMp;
            } else
            {
                CurrentMp = mp;
            }
        }
    }
}
