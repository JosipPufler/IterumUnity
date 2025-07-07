using Assets.Scripts.GameLogic.models.enums;
using Iterum.models.enums;
using Iterum.models.items;
using Iterum.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models.interfaces
{
    public abstract class ICreature : IGameObject, IActionable, ISpellcaster, IDamageable, IContainer, ITargetable
    {
        public ICreature(IRace race, string name, Vector3Int position, string imagePath = null)
        {
            ID = Guid.NewGuid().ToString();
            Race = race;
            Name = name;
            CurrentPosition = position;
            IsDead = false;

            ModifierManager = new ModifierManager(this);
            ClassManager = new ClassManager(this);
            ProficiencyManager = new ProficiencyManager(this);
            WeaponSet = new WeaponSet(this);
            ArmorSet = new ArmorSet(this);

            CurrentAp = OriginalMaxAp;
            CurrentSanity = OriginalMaxSanity;
            CurrentHp = OriginalMaxHp;
            CurrentMp = OriginalMaxMp;
            ImagePath = imagePath;
        }

        public string ID { get; }
        public string ImagePath { get; set; }
        public IRace Race { get; }
        public ClassManager ClassManager { get; }
        public ModifierManager ModifierManager { get; }
        public ProficiencyManager ProficiencyManager { get; }
        public WeaponSet WeaponSet { get; }
        public ArmorSet ArmorSet { get; }

        public Vector3Int CurrentPosition {  get; set; }
        public string Name {  get; set; }

        public bool IsDead { get; set; }
        
        public int CurrentSanity { get; set; }

        public int CurrentAp { get; set; }

        public int CurrentMp { get; set; }

        public IDictionary<WeaponSlot, int> GetWeaponSlots() {
            return ModifierManager.WeaponSlots.ToArray().Concat(Race.WeaponSlots).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(x => x.Value));
        }

        public IDictionary<ArmorSlot, int> GetArmorSlots()
        {
            return ModifierManager.ArmorSlots.ToArray().Concat(Race.ArmorSlots).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(x => x.Value));
        }

        public RollType GetTargetRollType() { 
            return RollType.Normal;
        }

        public int GetInitiative() { 
            return ModifierManager.GetAttribute(Attribute.Initiative, false) + ModifierManager.GetAttribute(Attribute.Agility, false);
        }

        public int Skillcheck(Skill skill, RollType rollType) {
            int result = DiceUtils.Roll(Dice.d20, rollType);
            return result + ModifierManager.GetAttribute(skill.Attribute, false) + ModifierManager.GetAttribute(skill.Stat.Attribute, false) + ProficiencyManager.GetSkillProficiencyBonus(skill);
        }

        public int SavingThrow(Stat stat, RollType rollType)
        {
            int result = DiceUtils.Roll(Dice.d20, rollType);
            return result + ModifierManager.GetAttribute(stat.Attribute, false) + ProficiencyManager.GetSavingThrowProficiencyBonus(stat);
        }

        public IList<IAction> GetActions()
        {
            var actions = new List<IAction>();

            actions.AddRange(Race.GetActions());
            actions.AddRange(WeaponSet.GetActions());
            actions.AddRange(ArmorSet.GetActions());
            actions.AddRange(ClassManager.GetClassActions());

            return actions;
        }

        public void TakeDamage(IEnumerable<DamageResult> damage) {
            var sources = new List<IResistable>();

            sources.AddRange(ArmorSet.GetArmor());
            sources.AddRange(ClassManager.GetAllClasses());
            sources.Add(Race);

            IDictionary<DamageType, double> resistances = DamageUtils.CalculateEfectiveDamage(sources);
            foreach (DamageResult damageResult in damage)
            {
                if (resistances.TryGetValue(damageResult.DamageType, out double resistance))
                {
                    if (damageResult.DamageType.DamageCategory.DamageClass == DamageClass.Health)
                    {
                        CurrentHp -= (int)Math.Ceiling(damageResult.Amount * resistance);
                    }
                    else
                    {
                        CurrentSanity -= (int)Math.Ceiling(damageResult.Amount * resistance);
                    }
                }
            }
            if (CurrentSanity <= 0)
            {
                CurrentSanity = 0;
                GoInsane();
            }
        }

        public void GoInsane()
        {
            Debug.Log("IM INSANE");
        }

        public int EvasionRating 
        {
            get 
            {
                return 10 + ModifierManager.GetAttribute(Attribute.Agility, false) + ModifierManager.GetAttribute(Attribute.EvasionRating, false) + ArmorSet.GetEvasionRatingModifier() + WeaponSet.GetEvasionRatingBonus();
            } 
            set 
            {
                ModifierManager.SetModifier(Attribute.EvasionRating, value - (10 + ModifierManager.GetAttribute(Attribute.Agility, false) + ModifierManager.GetAttribute(Attribute.EvasionRating, true)));
            }
        }

        public int Initiative
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.Agility, false) + ModifierManager.GetAttribute(Attribute.Initiative, false);
            }
            set
            {
                ModifierManager.SetModifier(Attribute.EvasionRating, value - (ModifierManager.GetAttribute(Attribute.Agility, false) + ModifierManager.GetAttribute(Attribute.Initiative, true)));
            }
        }

        public int OriginalMaxHp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxHp, true);
            }
        }

        public int MaxHp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxHp, false);
            }
            set
            {
                if (value < CurrentHp)
                {
                    CurrentHp = value;
                }
                ModifierManager.SetModifier(Attribute.MaxHp, value - OriginalMaxHp);
            }
        }

        public void Heal(int ammount) {
            ((IDamageable)this).Heal(ammount);
        }
        

        public int OriginalMaxSanity
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxSanity, true);
            }
        }

        public int MaxSanity
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxHp, false);
            }
            set
            {
                if (value < CurrentSanity)
                {
                    CurrentSanity = value;
                }
                ModifierManager.SetModifier(Attribute.MaxSanity, value - OriginalMaxSanity);
            }
        }

        public int OriginalMaxMp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxMp, true);
            }
        }

        public int MaxMp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxMp, false);
            }
            set
            {
                if (value < CurrentMp)
                {
                    CurrentMp = value;
                }
                ModifierManager.SetModifier(Attribute.MaxMp, value - OriginalMaxMp);
            }
        }

        public int OriginalMaxAp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxAp, true);
            }
        }

        public int MaxAp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxAp, false);
            }
            set
            {
                if (value < CurrentAp)
                {
                    CurrentAp = value;
                }
                ModifierManager.SetModifier(Attribute.MaxAp, value - OriginalMaxAp);
            }
        }

        public int OriginalApRegen 
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.ApRegen, true);
            }
        }

        public int ApRegen
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.ApRegen, false);
            }
            set
            {
                ModifierManager.SetModifier(Attribute.ApRegen, value - OriginalApRegen);
                ApRegen = value;
            }
        }

        public TargetType TargetType
        {
            get
            {
                return TargetType.Creature;
            }
        }

        public IDictionary<IItem, int> Inventory { get; } = new Dictionary<IItem, int>();

        public int CurrentHp { get; set; }

        public Corpse Die() {
            IsDead = true;
            for (int i = WeaponSet.Weapons.Count - 1; i >= 0; i--)
            {
                WeaponSet.UnequipWeapon(Inventory, WeaponSet.Weapons.ElementAt(i));
            }
            foreach (var item in ArmorSet.GetArmor())
            {
                ArmorSet.DoffArmor(Inventory, item);
            }

            return new Corpse(this);
        }

        public string GetStatString() {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"HP: {CurrentHp}/{MaxHp}");
            stringBuilder.AppendLine($"AP: {CurrentAp}/{MaxAp}");
            if (MaxMp > 0 || OriginalMaxMp > 0)
            {
                stringBuilder.AppendLine($"MP: {CurrentMp}/{MaxMp}");
            }
            return stringBuilder.ToString();
        }

        public int RollInitiative(RollType rollType = RollType.Normal) {
            return DiceUtils.Roll(Dice.d20, rollType) + GetInitiative();
        }

        public static string DisplayName { get; private set; }

        public string GetToolTipText() {
            return $"{Name}\n{CurrentHp}/{MaxHp}HP ({Math.Round((double)CurrentHp / MaxHp, 2)*100}%)";
        }

        public int GetAttackModifier(Stat stat, AttackType type) {
            if (type == AttackType.Spell)
            {
                return ModifierManager.GetAttribute(stat.Attribute, false) + ModifierManager.GetAttribute(Attribute.SpellAttackModifier, false);
            }
            return ModifierManager.GetAttribute(stat.Attribute, false) + ModifierManager.GetAttribute(Attribute.WeaponAttackModifier, false);
        }
    }
}
