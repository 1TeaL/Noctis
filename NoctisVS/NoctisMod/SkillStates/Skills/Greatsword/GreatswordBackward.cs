using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using System.Runtime.CompilerServices;
using NoctisMod.Modules;
using System.Reflection;

namespace NoctisMod.SkillStates
{
    public class GreatswordBackward : BaseSkillState
    {
        float baseDuration = 0.7f;
        private Animator animator;
        private float chargePercent;
        private float maxCharge = StaticValues.GSMaxCharge;
        private float damageMult;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            base.StartAimMode(this.baseDuration, false);
            this.animator.SetBool("releaseChargeSlash", false);
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);

            base.PlayCrossfade("FullBody, Override", "GSCharge", "Attack.playbackRate", this.baseDuration, 0.05f);
        }

        public void ChargeCalc()
        {
            this.damageMult = StaticValues.GSChargeDamage + StaticValues.GSChargeMultiplier * (this.chargePercent * StaticValues.GSChargeDamage);            
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge < maxCharge)
            {
                if (inputBank.skill1.down && skillLocator.primary.skillDef == Noctis.greatswordSkillDef)
                {
                    this.chargePercent = base.fixedAge * attackSpeedStat / this.maxCharge;
                    ChargeCalc();

                }
                else if (inputBank.skill2.down && skillLocator.secondary.skillDef == Noctis.greatswordSkillDef)
                {
                    this.chargePercent = base.fixedAge * attackSpeedStat / this.maxCharge;
                    ChargeCalc();
                    

                }
                else if (inputBank.skill4.down && skillLocator.special.skillDef == Noctis.greatswordSkillDef)
                {
                    this.chargePercent = base.fixedAge * attackSpeedStat / this.maxCharge;
                    ChargeCalc();                    

                }
                else
                {
                    GreatswordBackward2 GreatswordBackward2 = new GreatswordBackward2();
                    GreatswordBackward2.damageMult = damageMult;
                    this.outer.SetNextState(GreatswordBackward2);
                    this.animator.SetBool("releaseChargeSlash", true);
                    return;
                }
            }
            else
            {
                GreatswordBackward2 GreatswordBackward2 = new GreatswordBackward2();
                GreatswordBackward2.damageMult = damageMult;
                this.outer.SetNextState(GreatswordBackward2);
                this.animator.SetBool("releaseChargeSlash", true);
                return;
            }

        }


        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.damageMult);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.damageMult = reader.ReadInt64();
        }
    }
}



