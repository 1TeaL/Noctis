using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using NoctisMod.Modules;

namespace NoctisMod.SkillStates
{
    public class PolearmForward : BaseMeleeAttack
    {

        public HurtBox Target;
        private Vector3 direction;

        public bool isTarget;

        private bool keepMoving;
        private float rollSpeed;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.polearmDashSpeed*2f;
        private float finalSpeedCoefficient = 0f;


        public override void OnEnter()
        {

            weaponDef = Noctis.polearmSkillDef;
            this.hitboxName = "PolearmThrustHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = StaticValues.polearmDamage;
            this.procCoefficient = StaticValues.polearmProc;
            this.pushForce = 500f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 2f;
            this.attackStartTime = 0.54f;
            this.attackEndTime = 0.77f;
            this.baseEarlyExitTime = 0.77f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "PolearmSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingDown";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            SpeedCoefficient = initialSpeedCoefficient * attackSpeedStat;
            this.direction = base.GetAimRay().direction.normalized;
            base.OnEnter();
            attackAmount += StaticValues.polearmExtraHit;
          
            keepMoving = true;

        }
        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            this.rollSpeed = num * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / (base.baseDuration));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.stopwatch > (this.baseDuration * this.attackStartTime) && keepMoving)
            {
                RecalculateRollSpeed();
                if (base.isAuthority)
                {
                    Vector3 velocity = direction.normalized * rollSpeed;
                    base.characterMotor.velocity = velocity;
                    base.characterDirection.forward = base.characterMotor.velocity.normalized;
                }                


            }

        }


        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "PolearmChargingThrust", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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
                this.outer.SetNextState(new PolearmCombo());
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



