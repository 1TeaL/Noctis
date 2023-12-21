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
using R2API.Networking;
using EntityStates.Huntress;

namespace NoctisMod.SkillStates
{
    public class GreatswordSwapAerial : BaseMeleeAttack
    {
        private HurtBox target;
        private bool isTarget;

        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.greatswordSkillDef;
            this.hitboxName = "AOEHitbox";

            this.damageType = DamageType.Stun1s;

            this.damageCoefficient = StaticValues.GSDamage;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.baseDuration = 0.5f;
            this.attackStartTime = 0.25f;
            this.attackEndTime = 0.85f;
            this.baseEarlyExitTime = 0.5f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "GreatswordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingUp";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffectMedium;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;


            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.Motor.RebuildCollidableLayers();

            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 1);

            base.OnEnter();
            hasVulnerability = true;

            if (isSwapped)
            {
                this.baseDuration = 0.375f;
                this.attackStartTime = 0f;
                this.attackEndTime = 0.6f;
                this.baseEarlyExitTime = 0.3f;
            }

            base.SmallHop(characterMotor, 20f);

            if(noctisCon.Target)
            {
                target = noctisCon.GetTrackingTarget();
                isTarget = true;
            }
            
            if(isTarget)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.Motor.SetPositionAndRotation(target.healthComponent.body.transform.position - base.GetAimRay().direction, Quaternion.LookRotation(base.GetAimRay().direction), true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void PlayAttackAnimation()
        {

            if (isSwapped)
            {
                animator.Play("FullBody, Override.GSDP", -1, 0.25f);
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "GSDP", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
            }
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


                GreatswordCombo GreatswordCombo = new GreatswordCombo();
                this.outer.SetNextState(GreatswordCombo);
                return;

            }


        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 0);

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

    }
}



