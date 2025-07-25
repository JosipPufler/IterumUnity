using Assets.DTOs;
using Assets.Scripts;
using Assets.Scripts.GameLogic.models.actions;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.interfaces;
using Assets.Scripts.GameLogic.models.races;
using Iterum.models.enums;
using Iterum.models.items;
using Iterum.Scripts.Utils.Managers;
using Iterum.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using Attribute = Iterum.models.enums.Attribute;

namespace Iterum.models.interfaces
{
    [Serializable]
    [UnityEngine.Scripting.Preserve]
    public class BaseCreature : IGameObject, IActionable, ISpellcaster, IDamageable, IContainer, ITargetable
    {
        private const int movementPerAp = 2;

        public BaseCreature(BaseRace race, string name, string imagePath = "Textures/default", string description = "")
        {
            ID = Guid.NewGuid().ToString();
            Race = race;
            Name = name;
            DisplayName = name;
            CurrentPosition = new GridCoordinate();
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
            Description = description;
        }

        public BaseCreature(){
            ModifierManager = new ModifierManager();
            ClassManager = new ClassManager();
            ProficiencyManager = new ProficiencyManager();
            WeaponSet = new WeaponSet();
            ArmorSet = new ArmorSet();
            CurrentPosition = new GridCoordinate();
            IsDead = false;
        }

        [OnDeserializing]
        protected void ClearDefaultWeapons(StreamingContext ctx)
        {
            WeaponSet?.Weapons.Clear();
        }

        [OnDeserialized]
        public void InitHelpers(StreamingContext context) { 
            ModifierManager.creature = this;
            ClassManager.creature = this;
            ProficiencyManager.creature = this;
            WeaponSet.SetCreature(this);
            ArmorSet.SetCreature(this);
        }

        public void Spawn() {
            CurrentAp = MaxAp;
            CurrentHp = MaxHp;
            CurrentMp = MaxMp;
            CurrentSanity = MaxSanity;
        }

        public void GetCustomActions(Action onComplete)
        {
            CustomActions.Clear();
            int remaining = CustomActionIds.Count;

            if (remaining == 0)
            {
                onComplete?.Invoke();
                return;
            }

            foreach (var id in CustomActionIds)
            {
                ActionManager.Instance.GetAction(
                    id,
                    actionDto =>
                    {
                        CustomActions.Add(actionDto.MapToCustomAction());

                        if (--remaining == 0)
                            onComplete?.Invoke();
                    },
                    err =>
                    {
                        Debug.LogError($"Error getting custom action {id}: {err}");
                        if (--remaining == 0)
                            onComplete?.Invoke();
                    }
                );
            }
        }

        public string ID { get; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public bool IsPlayer { get; set; }
        public string CharacterId { get; set; }
        [JsonProperty]
        public BaseRace Race { get; private set; }
        [JsonProperty]
        public ClassManager ClassManager { get; private set; }
        [JsonProperty]
        public ModifierManager ModifierManager { get; private set; }
        [JsonProperty]
        public ProficiencyManager ProficiencyManager { get; private set; }
        [JsonProperty]
        public WeaponSet WeaponSet { get; private set; }
        [JsonProperty]
        public ArmorSet ArmorSet { get; private set; }

        public GridCoordinate CurrentPosition {  get; set; }
        public string Name {  get; set; }
        public bool IsDead { get; set; }

        public int CurrentHp { get; set; }
        public int CurrentSanity { get; set; }
        public int CurrentAp { get; set; }
        public int CurrentMp { get; set; }
        public int MovementPoints { get; set; } = 0;

        public List<string> CustomActionIds { get; set; } = new();
        public List<CustomBaseAction> CustomActions { get; set; } = new();

        public void SetBaseStats(Dictionary<Stat, int> stats) {
            foreach (var stat in stats)
            {
                ModifierManager.BaseAttributes[stat.Key.Attribute] = stat.Value - 5; 
            }
        }

        public IDictionary<WeaponSlot, int> GetWeaponSlots()
        {
            return ModifierManager.WeaponSlots.ToArray().Concat(Race.WeaponSlots).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(x => x.Value));
        }

        public IDictionary<ArmorSlot, int> GetArmorSlots()
        {
            var allSlots = ModifierManager.ArmorSlots.Concat(Race.ArmorSlots);
            return allSlots
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Sum(kv => kv.Value));
        }

        public RollType GetTargetRollType() { 
            return RollType.Normal;
        }

        public int GetInitiative() { 
            return ModifierManager.GetAttribute(Attribute.Initiative, false) + ModifierManager.GetAttribute(Attribute.Agility, false);
        }

        public int Skillcheck(Skill skill, RollType rollType) {
            int result = DiceUtils.Roll(Dice.d20, rollType);
            return result + GetSkillModifier(skill);
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
            actions.AddRange(CustomActions);

            return actions;
        }

        public int TakeDamage(IEnumerable<DamageResult> damage) {
            Debug.Log(damage.First().Amount);
            var sources = new List<IResistable>();
            int damageTaken = 0;
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
                        damageTaken += (int)Math.Ceiling(damageResult.Amount * resistance);
                        CurrentHp -= (int)Math.Ceiling(damageResult.Amount * resistance);
                    }
                    else
                    {
                        CurrentSanity -= (int)Math.Ceiling(damageResult.Amount * resistance);
                    }
                }
                else
                {
                    CurrentHp -= damageResult.Amount;
                    damageTaken += damageResult.Amount;
                }
            }
            if (CurrentSanity <= 0)
            {
                CurrentSanity = 0;
                GoInsane();
            }
            if (CurrentHp <= 0)
            {
                CurrentHp = 0;
                Die();
            }
            return damageTaken;
        }

        public void GoInsane()
        {
            Debug.Log("IM INSANE");
        }

        [JsonIgnore]
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

        [JsonIgnore]
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

        [JsonIgnore]
        public int OriginalMaxHp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxHp, true);
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public int OriginalMaxSanity
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxSanity, true);
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public int OriginalMaxMp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxMp, true);
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public int OriginalMaxAp
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.MaxAp, true);
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public int OriginalApRegen 
        {
            get
            {
                return ModifierManager.GetAttribute(Attribute.ApRegen, true);
            }
        }

        [JsonIgnore]
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

        public int EffectiveMovementPoints {
            get => MovementPoints + GetMovementPerPoint() * CurrentAp; 
        }

        [JsonIgnore]
        public TargetType TargetType
        {
            get
            {
                return TargetType.Creature;
            }
        }

        public List<BaseItem> Inventory { get; } = new List<BaseItem>();

        public List<BaseConsumable> GetConsumables() {
            return Inventory.OfType<BaseConsumable>().ToList();
        }

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
            stringBuilder.AppendLine($"Sanity: {CurrentSanity}/{MaxSanity}");
            stringBuilder.AppendLine($"AP: {CurrentAp}/{MaxAp}");
            stringBuilder.AppendLine($"MP: {CurrentMp}/{MaxMp}");
            if (MovementPoints > 0)
            {
                stringBuilder.AppendLine($"Movement: {MovementPoints}");
            }
            stringBuilder.AppendLine($"Efective Movement: {EffectiveMovementPoints}");
            return stringBuilder.ToString();
        }

        public int RollInitiative(RollType rollType = RollType.Normal) {
            return DiceUtils.Roll(Dice.d20, rollType) + GetInitiative();
        }

        public static string DisplayName { get; private set; }

        public string GetToolTipText() {
            return $"{Name}\n{CurrentHp}/{MaxHp}HP ({Math.Round((double)CurrentHp / MaxHp, 2)*100}%)";
        }

        public int GetSaveDC(SavingThrow savingThrow) {
            return savingThrow.BaseDC + ModifierManager.GetAttribute(savingThrow.OriginStat.Attribute) + ProficiencyManager.GetProficiencyBonus();
        }

        public bool CreateMovementPoint() {
            if (CurrentAp > 0)
            {
                CurrentAp--;
                MovementPoints += GetMovementPerPoint();
                return true;
            }
            return false;
        }

        private int GetMovementPerPoint()
        {
            return movementPerAp + ModifierManager.GetAttribute(Attribute.WalkingSpeed, false);
        }

        public void EndTurn() {
            MovementPoints = 0;
            if (CurrentAp > MaxAp)
            {
                CurrentAp = MaxAp;
            }
        }

        public void StartTurn() {
            CurrentAp += ApRegen;
            if (CurrentAp > MaxAp)
            {
                CurrentAp = MaxAp;
            }
        }

        public int GetAttributeModifier(Attribute attribute, bool original = false) { 
            return ModifierManager.GetAttribute(attribute, original);
        }

        public int GetSkillModifier(Skill skill, bool original = false) {
            return ModifierManager.GetAttribute(skill.Attribute, original) + ModifierManager.GetAttribute(skill.Stat.Attribute, original) + ProficiencyManager.GetSkillProficiencyBonus(skill);
        }
    }
}
