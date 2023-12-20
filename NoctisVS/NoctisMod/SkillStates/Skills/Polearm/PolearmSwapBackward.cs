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
    public class PolearmSwapBackward : BaseMeleeAttack
    {
        private Vector3 direction;

        private float rollSpeed;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.polearmDashSpeed;
        private float finalSpeedCoefficient = 1f;


        public override void OnEnter()
        {

            isSwapped = noctisCon.isSwapped;
            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.polearmSkillDef;
            this.hitboxName = "PolearmThrustHitbox";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = StaticValues.polearmDamage;
            this.procCoefficient = StaticValues.polearmProc;
            this.pushForce = 1000f;
            this.baseDuration = 1.2f;
            this.attackStartTime = 0.15f;
            this.attackEndTime = 0.5f;
            this.baseEarlyExitTime = 0.5f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 10f;

            this.swingSoundString = "PolearmSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSlashStab";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;
            SpeedCoefficient = initialSpeedCoefficient * attackSpeedStat;
            this.direction = -base.GetAimRay().direction.normalized;
            this.direction.y = 0f;

            if (base.characterBody)
            {
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            base.OnEnter();
            attackAmount += StaticValues.polearmSwapExtraHit;
          

        }
        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            this.rollSpeed = num * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / (base.baseDuration * attackEndTime));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.stopwatch < (this.baseDuration * this.attackEndTime))
            {
                RecalculateRollSpeed();
                Vector3 velocity = this.direction * rollSpeed;
                velocity.y = 0;
                base.characterMotor.velocity = velocity;
                //base.characterDirection.forward = base.characterMotor.velocity.normalized;


            }

        }


        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "PolearmStabBackjump", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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



