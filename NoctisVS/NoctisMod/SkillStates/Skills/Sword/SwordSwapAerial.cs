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
    public class SwordSwapAerial : BaseMeleeAttack
    {
        public HurtBox Target;
        private Vector3 stillPosition;


        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.swordSkillDef;
            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = StaticValues.swordDamage;
            this.procCoefficient = StaticValues.swordProc;
            this.pushForce = 400f;
            this.bonusForce = new Vector3(0f, 1000f, 0f);
            this.baseDuration = 0.9f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.6f;
            this.baseEarlyExitTime = 0.6f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 8f;


            if (swingIndex == 0)
            {
                this.baseDuration = 0.9f;
                this.attackStartTime = 0.3f;
                this.attackEndTime = 0.6f;
                this.baseEarlyExitTime = 0.6f;
            }
            else if (swingIndex == 1)
            {
                this.baseDuration = 1f;
                this.attackStartTime = 0.25f;
                this.attackEndTime = 0.6f;
                this.baseEarlyExitTime = 0.6f;
            }
            else if (swingIndex == 2)
            {
                this.baseDuration = 1f;
                this.attackStartTime = 0.1f;
                this.attackEndTime = 0.45f;
                this.baseEarlyExitTime = 0.45f;
            }

            this.swingSoundString = "SwordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = ChooseMuzzleString();
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;


            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1);

            stillPosition = Target.healthComponent.body.transform.position;

            base.OnEnter();

            if (isSwapped && swingIndex == 0)
            {
                this.baseDuration = 0.7f;
                this.attackStartTime = 0f;
                this.attackEndTime = 0.6f;
                this.baseEarlyExitTime = 0.3f;
            }

            if(base.isAuthority && Vector3.Distance(stillPosition, characterBody.corePosition) > 5f)
            {
                //base.characterMotor.Motor.SetPositionAndRotation(stillPosition + Vector3.up * 2f, Quaternion.LookRotation(base.GetAimRay().direction), true);

                base.characterMotor.Motor.SetPositionAndRotation(stillPosition - characterDirection.forward * 2f + Vector3.up, Quaternion.LookRotation(base.GetAimRay().direction), true);
            }

            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);
        }

        private string ChooseMuzzleString()
        {
            string returnVal = "SwordSwingRight";
            switch (this.swingIndex)
            {
                case 0:
                    returnVal = "SwordSwingRight";
                    break;
                case 1:
                    returnVal = "SwordSwingDown";
                    break;
                case 2:
                    returnVal = "SwordSwingRight";
                    break;
            }

            return returnVal;
        }

        public override void Update()
        {
            base.Update();

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

        protected override void PlayAttackAnimation()
        {
            
            if(swingIndex == 0)
            {

                AkSoundEngine.PostEvent("Dodge", base.gameObject);
                if (isSwapped)
                {
                    animator.Play("FullBody, Override.AerialSwordSlash", 1, 0.29f);
                }
                else
                {
                    base.PlayCrossfade("FullBody, Override", "AerialSwordSlash", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
                }
            }
            else if(swingIndex == 1)
            {
                base.PlayCrossfade("FullBody, Override", "AerialSwordSlash2", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
            }
            else if (swingIndex == 2)
            {
                base.PlayCrossfade("FullBody, Override", "AerialSwordSlash3", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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
                                
                if(swingIndex < 2)
                {
                    SwordSwapAerial SwordSwapAerial = new SwordSwapAerial();
                    int index = this.swingIndex;
                    index += 1;
                    SwordSwapAerial.swingIndex = index;
                    SwordSwapAerial.Target = Target;
                    this.outer.SetNextState(SwordSwapAerial);
                    return;
                }
                else
                {
                    if (!this.hasFired) this.FireAttack();
                    this.outer.SetNextState(new SwordCombo());
                    return;
                }
                

            }


        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0);

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

    }
}



