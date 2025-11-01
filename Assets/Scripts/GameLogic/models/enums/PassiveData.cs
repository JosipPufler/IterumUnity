using Iterum.models;
using Iterum.models.interfaces;

namespace Assets.Scripts.GameLogic.models.enums
{
    public interface IPassiveData
    {
        PassiveTrigger Trigger { get; }
    }

    public record GetAttributeData(BaseCreature Creature, object AdditionalData = null) : IPassiveData {
        public PassiveTrigger Trigger => PassiveTrigger.GetAttribute;
    }
    public record DeathData(BaseCreature Creature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.Death;
    }
    public record LongRestData(BaseCreature Creature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.LongRest;
    }
    public record ShortRestData(BaseCreature Creature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.ShortRest;
    }
    public record DamageDealtData(BaseCreature Creature, DamageInfo Damage, BaseCreature TargetCreature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.DamageDelt;
    }
    public record DamageTakenData(BaseCreature Creature, DamageInfo Damage, BaseCreature OriginCreature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.DamageTaken;
    }
    public record AttackLaunchData(BaseCreature Creature, DamageInfo Damage, BaseCreature TargetCreature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.AttackLaunched;
    }
    public record AttackHitData(BaseCreature Creature, DamageInfo Damage, BaseCreature OriginCreature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.AttackLanded;
    }
    public record HealTargetData(BaseCreature Creature, HealInfo Damage, BaseCreature TargetCreature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.HealTarget;
    }
    public record HealReceivedData(BaseCreature Creature, HealInfo Damage, BaseCreature OriginCreature, object AdditionalData = null) : IPassiveData
    {
        public PassiveTrigger Trigger => PassiveTrigger.HealReceived;
    }
}
