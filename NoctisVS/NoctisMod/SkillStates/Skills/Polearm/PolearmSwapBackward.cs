using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using EntityStates.VoidSurvivor;
using NoctisMod.Modules;
using System;

namespace NoctisMod.SkillStates
{
    public class PolearmSwapBackward : BaseSkillState
    {
        public bool isSwapped;
        private NoctisController noctisCon;
        private Animator animator;
        public float duration = 2f;
        public float earlyExitTime = 0.6f;
        private float fireTime = 0.5f;
        public bool hasFired;
        private Ray aimRay;
        private float range = 200f;
        private float damageCoefficient = Modules.StaticValues.polearmSwapBackwardDamage;
        private float procCoefficient = Modules.StaticValues.polearmProc;
        private string muzzleString ="RHand";
        private int bulletcount;

        public override void OnEnter()
        {
            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            noctisCon.WeaponAppearR(3f, NoctisController.WeaponTypeR.POLEARMR);
            isSwapped = noctisCon.isSwapped;
            hasFired = false;
            aimRay = base.GetAimRay();
            base.StartAimMode(this.duration, true);
            animator = base.GetModelAnimator();
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);
                       

            bulletcount = Mathf.RoundToInt(attackSpeedStat) + StaticValues.polearmSwapExtraHit;
            base.characterDirection.forward = aimRay.direction;

            if (isSwapped)
            {
                animator.Play("FullBody, Override.PolearmThrow", -1, 0.34f);
                this.duration = 2f;
                this.earlyExitTime = 0.4f;
                this.fireTime = 0.01f;
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "PolearmThrow", "Attack.playbackRate", duration, 0.05f);
            }

        }

        public override void OnExit()
        {
            base.OnExit();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterDirection.forward = aimRay.direction;
            if (base.fixedAge >= this.fireTime * duration && !hasFired)
            {
                noctisCon.currentWeaponR.SetActive(false);
                hasFired = true;
                AkSoundEngine.PostEvent("PolearmSwingSFX", base.gameObject);
                Ray aimRay = base.GetAimRay();
                EffectManager.SpawnEffect(Assets.polearmThrowParticle, new EffectData
                {
                    origin = aimRay.origin,
                    rotation = Quaternion.LookRotation(new Vector3(aimRay.direction.x, aimRay.direction.y, aimRay.direction.z)),
                }, true);

                var bulletAttack = new BulletAttack
                {
                    bulletCount = (uint)bulletcount,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = this.damageStat * damageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = range,
                    force =100f,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 0f,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 1.5f,
                    sniper = true,
                    stopperMask = LayerIndex.noCollision.mask,
                    weapon = null,
                    //tracerEffectPrefab = Assets.polearmTracer,
                    spreadPitchScale = 0f,
                    spreadYawScale = 0f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,

                };
                bulletAttack.Fire();
            }
            
            if(base.fixedAge > this.duration * earlyExitTime)
            {

                if (base.isAuthority)
                {
                    if (inputBank.skill1.down)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    if (inputBank.skill2.down)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    if (inputBank.skill3.down)
                    {
                        this.outer.SetNextState(new Dodge());
                        return;
                    }
                    if (inputBank.skill4.down)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    if (inputBank.jump.down)
                    {
                        Jump Jump = new Jump();
                        this.outer.SetNextState(Jump);
                        return;

                    }
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }




        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
