﻿using EntityStates;
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
    public class SwordSwapBackward : BaseMeleeAttack
    {
        public HurtBox Target;
        private Vector3 direction;

        public bool isTarget;

        private bool keepMoving;
        private float rollSpeed;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.swordDashSpeed;
        private float finalSpeedCoefficient = 0f;
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("ShiggyMelee", base.gameObject);

            weaponDef = Noctis.swordSkillDef;
            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;
            
            this.damageCoefficient = 1f;
            this.procCoefficient = 1f;
            this.pushForce = 0f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.5f;
            this.baseEarlyExitTime = 0.3f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "ShiggyMelee";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSwingUp";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1, 0);

            SpeedCoefficient = initialSpeedCoefficient * attackSpeedStat;
            this.direction = base.GetAimRay().direction.normalized;

            if (base.characterBody)
            {
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }

            base.SmallHop(base.characterMotor, StaticValues.dodgeHop);

            base.OnEnter();

        }


        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            this.rollSpeed = num * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / (base.baseDuration * this.attackEndTime));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.stopwatch <= (this.baseDuration * this.attackEndTime) && keepMoving)
            {
                RecalculateRollSpeed();
                if (isTarget)
                {
                    if (Target)
                    {
                        this.direction = Target.transform.position;
                    }
                    if (base.isAuthority)
                    {
                        Vector3 velocity = (base.transform.position - this.direction).normalized * rollSpeed;
                        base.characterMotor.velocity = velocity;
                        base.characterDirection.forward = base.characterMotor.velocity.normalized;
                    }

                }
                else
                {
                    if (base.isAuthority)
                    {
                        Vector3 velocity = (base.transform.position - this.direction).normalized * rollSpeed;
                        base.characterMotor.velocity = velocity;
                        base.characterDirection.forward = base.characterMotor.velocity.normalized;
                    }

                }


            }

        }


        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "SwordKickFlip", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();

        }

        protected override void SetNextState()
        {

            if (base.isAuthority)
            {
                if (!this.hasFired) this.FireAttack();

                this.outer.SetNextState(new SwordCombo());
                return;

            }


        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0, 0);
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.velocity *= 0.1f;

        }

    }
}



