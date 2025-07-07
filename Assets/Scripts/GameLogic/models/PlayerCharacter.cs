using Iterum.models.interfaces;
using System;
using System.Collections.Generic;

namespace Iterum.models
{
    /*internal class PlayerCharacter : IDownableCreature
    {
        public PlayerCharacter(IRace race) {
            ID = Guid.NewGuid().ToString();
            Race = race;
            IsDead = false;

            ClassManager = new ClassManager(this);
            ModifierManager = new ModifierManager(this);
            ProficiencyManager = new ProficiencyManager(this);
            WeaponSet = new WeaponSet(this);
            ArmorSet = new ArmorSet(this);

            CurrentAp = ((ICreature)this).OriginalMaxAp;
            CurrentSanity = ((ICreature)this).OriginalMaxSanity;
            CurrentHp = ((ICreature)this).OriginalMaxHp;
            CurrentMp = ((ICreature)this).OriginalMaxMp;
        }

        public IRace Race { get; private set; }

        public ClassManager ClassManager { get; private set; }

        public ModifierManager ModifierManager { get; private set; }

        public ProficiencyManager ProficiencyManager { get; private set; }

        public WeaponSet WeaponSet { get; private set; }

        public ArmorSet ArmorSet { get; private set; }

        public List<bool> DeathSaves { get; } = new List<bool>();

        public Position CurrentPosition { get; set; }

        public string Name { get; }
        public bool IsDown { get; set; } = false;
        public int CurrentAp { get; set; }
        public int CurrentMp { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentSanity { get; set; }

        public IList<IItem> Inventory { get; } = new List<IItem>();
        public bool IsDead { get; set; }
        public string ImagePath { get; set; }
        public string ID { get; }
    }*/
}
