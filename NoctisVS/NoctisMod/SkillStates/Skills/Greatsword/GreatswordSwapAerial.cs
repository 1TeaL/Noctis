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
using NoctisMod.Modules.Networking;
using R2API.Networking.Interfaces;

namespace NoctisMod.SkillStates
{
    public class GreatswordSwapAerial : BaseMeleeAttack
    {
        public CharacterBody target;
        private bool hasLaunched;

        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.greatswordSkillDef;
            this.hitboxName = "AOEHitbox";

            this.damageType = DamageType.Stun1s;

            this.damageCoefficient = 1f;
            this.procCoefficient = StaticValues.GSProc;
            this.pushForce = 1000f;
            this.bonusForce = new Vector3(0f, 5000f, 0f);
            this.baseDuration = 1f;
            this.attackStartTime = 0.25f;
            this.attackEndTime = 0.85f;
            this.baseEarlyExitTime = 0.8f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "GreatswordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingUp";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffectMedium;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;


            characterBody.ApplyBuff(Modules.Buffs.GSarmorBuff.buffIndex, 1);

            base.OnEnter();
            hasVulnerability = true;

            if (isSwapped)
            {
                this.baseDuration = 0.7f;
                this.attackStartTime = 0f;
                this.attackEndTime = 0.6f;
                this.baseEarlyExitTime = 0.3f;
            }

            if(base.isAuthority)
            {
                base.characterMotor.Motor.SetPositionAndRotation(target.healthComponent.body.transform.position - characterDirection.forward * 2f, Quaternion.LookRotation(base.GetAimRay().direction), true);
            }

            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.stopwatch >= (this.baseDuration * this.attackStartTime) && !hasLaunched)
            {
                hasLaunched = true;
                base.SmallHop(characterMotor, 20f);
                new TakeDamageRequest(characterBody.masterObjectId, target.masterObjectId, damageStat * StaticValues.GSDamage, Vector3.up, true, true).Send(NetworkDestination.Clients);
            }
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

                if (hasLaunched && target.healthComponent.health > 1f)
                {
                    new ForceFollowUpState(characterBody.masterObjectId, target.masterObjectId).Send(NetworkDestination.Clients);
                    return;

                }
                else
                {
                    GreatswordCombo GreatswordCombo = new GreatswordCombo();
                    this.outer.SetNextState(GreatswordCombo);
                    return;
                }

            }


        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(Modules.Buffs.GSarmorBuff.buffIndex, 0);

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

    }
}



