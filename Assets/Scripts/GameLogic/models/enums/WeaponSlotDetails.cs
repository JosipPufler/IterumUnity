namespace Iterum.models.enums
{
    public class WeaponSlotDetails
    {
        public WeaponSlotDetails(WeaponSlot slot, int numberOfSlotsRequired)
        {
            Slot = slot;
            SlotsNeeded = numberOfSlotsRequired;
        }

        public WeaponSlot Slot { get; set; }

        public int SlotsNeeded { get; set; }

        public static WeaponSlotDetails OneHand = new(WeaponSlot.Hand, 1);
        public static WeaponSlotDetails TwoHand = new(WeaponSlot.Hand, 2);
        public static WeaponSlotDetails Special = new(WeaponSlot.Special, 1);
        public static WeaponSlotDetails Natural = new(WeaponSlot.Special, 0);
    }
}
