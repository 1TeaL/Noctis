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
    public class PolearmSwapAerial : BaseMeleeAttack
    {

        public HurtBox Target;
        private Vector3 direction;

        public bool isTarget;

        private bool keepMoving;
        private float rollSpeed;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.polearmDashSpeed;
        private float finalSpeedCoefficient = 0f;


        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.polearmSkillDef;
            keepMoving = true;
            this.hitboxName = "PolearmThrustHitbox";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = StaticValues.polearmDamage;
            this.procCoefficient = StaticValues.polearmProc;
            this.pushForce = 0f;
            this.baseDuration = 0.9f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.6f;
            this.baseEarlyExitTime = 0.6f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 6f;

            this.swingSoundString = "PolearmSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSwingStab";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;
            SpeedCoefficient = initialSpeedCoefficient * attackSpeedStat;
            this.direction = base.GetAimRay().direction.normalized;

            if (base.characterBody)
            {
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            base.SmallHop(base.characterMotor, hitHopVelocity);
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


            }

        }


        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "AerialOneHandStab", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);            
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

                if(keepMoving)
                {
                    this.outer.SetNextState(new PolearmCombo());
                    return;
                }
                else if(!keepMoving)
                {
                    this.outer.SetNextState(new PolearmDoubleDragoonThrust());
                    return;
                }
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



