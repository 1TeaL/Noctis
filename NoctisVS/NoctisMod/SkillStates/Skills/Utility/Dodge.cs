using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using System.Reflection;
using R2API.Networking;
using NoctisMod.Modules;

namespace NoctisMod.SkillStates
{
    public class Dodge : BaseSkillState
    {
        private Animator animator;
        public NoctisController noctisCon;
        private Ray aimRay;
        private float rollSpeed;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.dodgeSpeed;
        private float finalSpeedCoefficient = 0.1f;

        private float baseDuration = 1f;
        private float duration;

        

        private Vector3 direction;


        public override void OnEnter()
        {

            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            this.animator = base.GetModelAnimator();
            aimRay = base.GetAimRay();
            noctisCon.weaponState = NoctisController.weaponType.NONE;

            direction = base.inputBank.moveVector;
            duration = baseDuration / attackSpeedStat;

            base.GetModelAnimator().SetFloat("Attack.playbackRate", attackSpeedStat);

            characterBody.ApplyBuff(Modules.Buffs.dodgeBuff.buffIndex, 1);
            if (!characterMotor.isGrounded)
            {
                base.SmallHop(base.characterMotor, StaticValues.dodgeHop);
            }
            PlayAnimation();
        }

        private void PlayAnimation()
        {
            if (!characterMotor.isGrounded)
            {
                base.PlayCrossfade("FullBody, Override", "AerialDodge", "Attack.playbackRate", this.duration, 0.05f);
            }
            else
            if (characterMotor.isGrounded)
            {
                base.PlayCrossfade("FullBody, Override", "Roll", "Attack.playbackRate", this.duration, 0.05f);
            }
        }

        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            this.rollSpeed = num * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(Modules.Buffs.dodgeBuff.buffIndex, 0);
        }

        public override void Update()
        {
            base.Update();
            RecalculateRollSpeed();
            Vector3 velocity = direction.normalized * rollSpeed;
            base.characterMotor.velocity = velocity;

            if (!characterMotor.isGrounded)
            {
                base.StartAimMode(0.5f, true);
            }
            else
            if (characterMotor.isGrounded)
            {
                base.characterDirection.forward = base.inputBank.moveVector;
            }
            
            if(base.fixedAge > duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }

        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}



