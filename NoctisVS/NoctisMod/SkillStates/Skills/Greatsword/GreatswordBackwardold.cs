﻿//using EntityStates;
//using RoR2;
//using UnityEngine;
//using NoctisMod.Modules.Survivors;
//using System;
//using System.Collections.Generic;
//using UnityEngine.Networking;
//using NoctisMod.SkillStates.BaseStates;
//using R2API;
//using System.Runtime.CompilerServices;
//using NoctisMod.Modules;
//using System.Reflection;
//using static NoctisMod.Modules.Survivors.NoctisController;
//using R2API.Networking;

//namespace NoctisMod.SkillStates
//{
//    public class GreatswordBackwardold : BaseSkillState
//    {
//        NoctisController noctisCon;
//        float baseDuration = 0.7f;
//        private Animator animator;
//        private float chargePercent;
//        private float maxCharge = StaticValues.GSMaxCharge;
//        private float damageMult;

//        public override void OnEnter()
//        {
//            base.OnEnter();
//            noctisCon = gameObject.GetComponent<NoctisController>();
//            this.animator = base.GetModelAnimator();
//            base.StartAimMode(this.baseDuration, false);
//            this.animator.SetBool("releaseChargeSlash", false);
//            this.animator.SetBool("releaseChargeLeap", false);
//            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);

//            base.PlayCrossfade("FullBody, Override", "GSCharge", "Attack.playbackRate", this.baseDuration, 0.05f);
//            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 1);
//        }

//        public void ChargeCalc()
//        {
//            this.chargePercent = base.fixedAge * attackSpeedStat / this.maxCharge;
//            this.damageMult = StaticValues.GSChargeDamage + StaticValues.GSChargeMultiplier * (this.chargePercent * StaticValues.GSChargeDamage);
//            noctisCon.WeaponAppearR(3f, WeaponTypeR.GREATSWORD);
//        }

//        public override void FixedUpdate()
//        {
//            base.FixedUpdate();
//            characterMotor.velocity = Vector3.zero;
//            if (base.fixedAge < maxCharge)
//            {
//                if (inputBank.skill1.down && skillLocator.primary.skillDef == Noctis.greatswordSkillDef)
//                {
//                    ChargeCalc();
//                }
//                else if (inputBank.skill2.down && skillLocator.secondary.skillDef == Noctis.greatswordSkillDef)
//                {
//                    ChargeCalc();
//                }
//                else if (inputBank.skill4.down && skillLocator.special.skillDef == Noctis.greatswordSkillDef)
//                {
//                    ChargeCalc();        
//                }
//                else
//                {
//                    GreatswordBackward GreatswordBackward2 = new GreatswordBackward();
//                    GreatswordBackward2.damageMult = damageMult;
//                    this.outer.SetNextState(GreatswordBackward2);
//                    this.animator.SetBool("releaseChargeSlash", true);
//                    return;
//                }
//            }
//            else
//            {
//                GreatswordBackward GreatswordBackward2 = new GreatswordBackward();
//                GreatswordBackward2.damageMult = damageMult;
//                this.outer.SetNextState(GreatswordBackward2);
//                this.animator.SetBool("releaseChargeSlash", true);
//                return;
//            }

//        }


//        public override void OnExit()
//        {
//            base.OnExit();
//        }

//        public override void OnSerialize(NetworkWriter writer)
//        {
//            base.OnSerialize(writer);
//            writer.Write(this.damageMult);
//        }

//        public override void OnDeserialize(NetworkReader reader)
//        {
//            base.OnDeserialize(reader);
//            this.damageMult = reader.ReadInt64();
//        }
//    }
//}



