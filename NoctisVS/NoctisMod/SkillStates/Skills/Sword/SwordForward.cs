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

namespace NoctisMod.SkillStates
{
    public class SwordForward : BaseMeleeAttack
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
            keepMoving = true;
            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = 1f;
            this.procCoefficient = 1f;
            this.pushForce = 1000f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.4f;
            this.attackEndTime = 0.8f;
            this.baseEarlyExitTime = 0.4f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "ShiggyMelee";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSlashNeutral{this.swingIndex + 1}";
            this.swingEffectPrefab = Modules.Assets.noctisHitEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;
            SpeedCoefficient = initialSpeedCoefficient * attackSpeedStat;

            if (base.characterBody)
            {
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
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
                        Vector3 velocity = (this.direction - base.transform.position).normalized * rollSpeed;
                        base.characterMotor.velocity = velocity;
                        base.characterDirection.forward = base.characterMotor.velocity.normalized;
                    }

                }
                else
                {
                    if (base.isAuthority)
                    {
                        base.characterDirection.forward = this.direction;
                        base.characterMotor.velocity = Vector3.zero;
                        base.characterMotor.rootMotion += this.direction * this.rollSpeed * Time.fixedDeltaTime;
                    }

                }


            }

        }


        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "SwordLeapSlash", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
            
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            keepMoving = false;

        }

        protected override void CheckNextState()
        {
            if (!this.hasFired) this.FireAttack();

            if (base.isAuthority && base.IsKeyDownAuthority())
            {
                this.outer.SetNextState(new SwordCombo());
                return;
            }

        }

        public override void OnExit()
        {
            base.OnExit();
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.velocity *= 0.1f;
        }

    }
}



