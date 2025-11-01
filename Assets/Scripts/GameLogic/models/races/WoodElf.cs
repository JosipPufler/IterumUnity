using Assets.Scripts.Utils;
using Iterum.models.enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic.models.races
{
    public class WoodElf : BaseRace
    {
        public override string Name { get; set; } = "Wood elf";
        public override string Description { get; set; } = "Wood elves dwell in the kingdom of Arboria, deep within the Great Woods. They are known for their agility, speed, and dexterity, as well as their six-fingered hands and unusually long arms, which help them climb and swing through the forest canopies they call home.";

        public override Dictionary<Attribute, int> RacialAttributes { get; protected set; } = new Dictionary<Attribute, int>() { { Attribute.Reach, 1 }, { Attribute.WalkingSpeed, 1} };

        public override HashSet<Skill> RacialSkills { get { 
                return new HashSet<Skill>() { Skill.SlightOfHand, Skill.Nature };
            } 
        }

        [JsonConverter(typeof(DictionaryKeyArmorSlotConverterInt))]
        public override Dictionary<ArmorSlot, int> ArmorSlots
        {
            get
            {
                return new Dictionary<ArmorSlot, int>() {
                    { ArmorSlot.Head, 1 },
                    { ArmorSlot.Torso, 1 },
                    { ArmorSlot.Hand, 2 },
                    { ArmorSlot.Legs, 1 },
                    { ArmorSlot.Ring, 12 },
                    { ArmorSlot.Necklace, 1 },
                    { ArmorSlot.Boots, 1 },
                };
            }
        }
    }
}
