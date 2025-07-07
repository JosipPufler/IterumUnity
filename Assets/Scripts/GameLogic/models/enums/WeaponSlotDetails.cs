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

        public static WeaponSlotDetails OneHand = new WeaponSlotDetails(WeaponSlot.Hand, 1);
        public static WeaponSlotDetails TwoHand = new WeaponSlotDetails(WeaponSlot.Hand, 2);
        public static WeaponSlotDetails Special = new WeaponSlotDetails(WeaponSlot.Special, 1);
        public static WeaponSlotDetails Natural = new WeaponSlotDetails(WeaponSlot.Special, 0);
    }
}
