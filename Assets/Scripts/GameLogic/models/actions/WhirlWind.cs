using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.GameLogic.models.items;
using Assets.Scripts.GameLogic.models.target;
using Assets.Scripts.GameLogic.utils;
using Iterum.models;
using Iterum.models.enums;
using Iterum.models.interfaces;
using Iterum.utils;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.GameLogic.models.actions
{
    public class WhirlWind : BaseAction
    {
        public WhirlWind()
        {
            Name = "Whirlwind";
            Description = "Swing a two handed weapon around you attacking all enemies around you";
            ApCost = 3;
            Initialize();
        }

        public override void Initialize()
        {
            Action = Func;
        }

        public override BaseAction Clone()
        {
            return new WhirlWind();
        }

        ActionResult Func(ActionInfo actionInfo)
        {
            ActionResultBuilder actionResultBuilder = ActionResultBuilder.Start(actionInfo.OriginCreature);
            BaseCreature originCreature = actionInfo.OriginCreature;
            // Maybe let user choose which weapon if multiple equpit somehow
            BaseWeapon weapon = originCreature.WeaponSet.Weapons.FirstOrDefault(weapon => weapon.WeaponSlotDetails.SlotsNeeded == 2 && weapon.WeaponSlotDetails.Slot == WeaponSlot.Hand);
            Debug.Log(weapon.Name);

            if (weapon != null)
            {
                Stat baseStat = Stat.Strength;
                if (weapon.WeaponTraits.Contains(WeaponTrait.Finesse) && originCreature.GetAttributeModifier(Attribute.Agility) > originCreature.GetAttributeModifier(Attribute.Strength))
                {
                    baseStat = Stat.Agility;
                }

                foreach (CharacterToken targetCreature in CampaignGridLayout.Instance.GetTokensInRing(actionInfo.OriginCreature.CurrentPosition, 1, 1)) {
                    foreach (var item in weapon.DamageInfos)
                    {
                        Debug.Log(item);
                    }
                    CombatUtils.Attack(originCreature, targetCreature.creature, AttackType.MeleeWeapon(baseStat, originCreature.ProficiencyManager.IsProficient(weapon)), new ActionPackage() { DamageOnSuccess = weapon.DamageInfos }, actionResultBuilder);
                }

                return actionResultBuilder.Build();
            }
            return actionResultBuilder.Fail().Build();
        }
    }
}
