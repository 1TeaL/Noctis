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
using NoctisMod.Modules;

namespace NoctisMod.SkillStates
{
    public class SwordForward : BaseMeleeAttack
    {
        private Vector3 direction;

        private bool keepMoving;
        private float rollSpeed;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.swordDashSpeed;
        private float finalSpeedCoefficient = 0f;


        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.swordSkillDef;
            keepMoving = true;
            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = StaticValues.swordDamage;
            this.procCoefficient = StaticValues.swordProc;
            this.pushForce = 300f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.65f;
            this.baseEarlyExitTime = 0.65f; 
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 10f;

            this.swingSoundString = "SwordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSlashDown";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;
            SpeedCoefficient = initialSpeedCoefficient * attackSpeedStat;
            this.direction = base.GetAimRay().direction.normalized;

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

            if (this.stopwatch <= (this.baseDuration * this.attackStartTime) && keepMoving)
            {
                RecalculateRollSpeed();
                Vector3 velocity = this.direction * rollSpeed;
                velocity.y = base.characterMotor.velocity.y;
                base.characterMotor.velocity = velocity;
                //base.characterDirection.forward = base.characterMotor.velocity.normalized;                


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
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.velocity *= 0.1f;
        }

    }
}



