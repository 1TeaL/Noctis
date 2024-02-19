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
    public class PolearmSwapAerial : BaseMeleeAttack
    {
        public HurtBox Target;
        private Vector3 stillPosition;
        private bool hasExtracted;


        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.polearmSkillDef;
            this.hitboxName = "PolearmThrustHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = 0f;
            this.procCoefficient = 0.1f;
            this.pushForce = 1000f;
            this.bonusForce = new Vector3(1000f, 500f, 0f);
            this.baseDuration = 1.5f;
            this.attackStartTime = 0.15f;
            this.attackEndTime = 0.35f;
            this.baseEarlyExitTime = 0.35f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 8f;

            this.swingSoundString = "PolearmSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSwingStab";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;



            stillPosition = Target.healthComponent.body.transform.position;

            base.OnEnter();

            if (isSwapped)
            {
                this.baseDuration = 1.26f;
                this.attackStartTime = 0f;
                this.attackEndTime = 0.24f;
                this.baseEarlyExitTime = 0.24f;
            }

            if(base.isAuthority)
            {
                //base.characterMotor.Motor.SetPositionAndRotation(stillPosition + Vector3.up * 2f, Quaternion.LookRotation(base.GetAimRay().direction), true);

                base.characterMotor.Motor.SetPositionAndRotation(stillPosition - characterDirection.forward * 3f + Vector3.up * 2f, Quaternion.LookRotation(base.GetAimRay().direction), true);
            }

            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);
        }

        public override void Update()
        {
            base.Update();

            if (base.age < attackEndTime)
            {
                //stop all movement during duration
                if (Target.healthComponent.body.characterMotor)
                {
                    Target.healthComponent.body.characterMotor.Motor.SetPositionAndRotation(stillPosition, Quaternion.LookRotation(base.GetAimRay().direction), true);
                }
                else if (Target.healthComponent.body.rigidbody)
                {
                    Target.healthComponent.body.rigidbody.MovePosition(stillPosition);
                }
                //base.characterMotor.Motor.SetPositionAndRotation(stillPosition + Vector3.up * 2f, Quaternion.LookRotation(base.GetAimRay().direction), true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge > attackStartTime && !hasExtracted)
            {
                hasExtracted = true;
                new ExtractNetworkRequest(characterBody.masterObjectId, Target.healthComponent.body.corePosition, 4f, damageStat * ((StaticValues.polearmDamage * (attackAmount + StaticValues.polearmSwapExtraHit)) + (StaticValues.polearmDamage * partialAttack))).Send(NetworkDestination.Clients);
            }
        }

        protected override void PlayAttackAnimation()
        {
            AkSoundEngine.PostEvent("Dodge", base.gameObject);
            if (isSwapped)
            {
                animator.Play("FullBody, Override.PolearmPullOut", 1, 0.16f);
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "PolearmPullOut", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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
                     
                if (!this.hasFired) this.FireAttack();
                this.outer.SetNextState(new PolearmCombo());
                return;
                
                

            }


        }

        public override void OnExit()
        {
            base.OnExit();

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

    }
}



