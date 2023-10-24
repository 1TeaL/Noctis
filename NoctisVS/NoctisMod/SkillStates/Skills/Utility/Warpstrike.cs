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
using R2API.Networking;
using NoctisMod.Modules;
using HG;
using NoctisMod.Modules.Networking;
using R2API.Networking.Interfaces;

namespace NoctisMod.SkillStates
{
    public class Warpstrike : BaseSkillState
    {
        private Animator animator;
        public NoctisController noctisCon;
        public EnergySystem energySystem;
        private Ray aimRay;
        private float rollSpeed;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.warpstrikeSpeed;
        private float finalSpeedCoefficient = 0.1f;
        public float dashSpeed = 100f;

        private float baseDuration = 0.6f;
        private float duration;
        private float warpStartTime = 0.4f;
        private float warpEndTime = 0.8f;
        private bool keepMoving;

        private Vector3 direction;
        private bool isTarget;
        private HurtBox Target;
        private float distance;
        private Vector3 storedPosition;

        public override void OnEnter()
        {

            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            energySystem = gameObject.GetComponent<EnergySystem>();

            //energy cost
            float manaflatCost = (StaticValues.warpstrikeCost) - (energySystem.costflatMana);
            if (manaflatCost < 0f) manaflatCost = StaticValues.minimumManaCost;

            float manaCost = energySystem.costmultiplierMana * manaflatCost;
            if (manaCost < 0f) manaCost = 0f;

            if (energySystem.currentMana < manaCost)
            {
                this.outer.SetNextStateToMain();
                return;
            }
            else if (energySystem.currentMana >= manaCost)
            {
                energySystem.SpendMana(manaCost);

            }

            isTarget = false;
            if (noctisCon.GetTrackingTarget())
            {
                Target = noctisCon.GetTrackingTarget();
                isTarget = true;
                distance = Vector3.Magnitude(Target.transform.position - base.characterBody.corePosition);
            }


            aimRay = base.GetAimRay();
            noctisCon.WeaponAppearR(0f, NoctisController.WeaponTypeR.NONE);
            keepMoving = true;

            direction = aimRay.direction.normalized;
            duration = baseDuration / attackSpeedStat;

            this.animator = base.GetModelAnimator();
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);
            SpeedCoefficient = initialSpeedCoefficient * attackSpeedStat;

            PlayAnimation();
            EffectManager.SpawnEffect(Assets.swordThrowParticle, new EffectData
            {
                origin = aimRay.origin,
                rotation = Quaternion.LookRotation(new Vector3(aimRay.direction.x, aimRay.direction.y, aimRay.direction.z)),
            }, true);
        }

        private void PlayAnimation()
        {
            if (isTarget)
            {
                base.PlayAnimation("FullBody, Override", "WarpStrike", "Attack.playbackRate", this.duration);
            }
            else
            {
                base.PlayAnimation("FullBody, Override", "WarpStrikeRoll", "Attack.playbackRate", this.duration);
            }
        }


        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.Update();                   

            if (base.fixedAge >= duration * warpStartTime && base.fixedAge <= duration * warpEndTime && keepMoving)
            {
                if(isTarget)
                {
                    this.storedPosition = Target.transform.position;
                    if(base.isAuthority)
                    {
                        Vector3 velocity = (this.storedPosition - base.transform.position).normalized * dashSpeed;
                        base.characterMotor.velocity = velocity;
                        base.characterDirection.forward = base.characterMotor.velocity.normalized;
                    }
                    Vector3 position = base.characterBody.corePosition + aimRay.direction.normalized * 1f;
                    float radius = 0.3f;
                    LayerIndex layerIndex = LayerIndex.world;
                    int num = layerIndex.mask;
                    layerIndex = LayerIndex.entityPrecise;
                    int num2 = Physics.OverlapSphere(position, radius, num | layerIndex.mask).Length;
                    bool flag2 = num2 != 0;
                    if (flag2)
                    {
                        new TakeDamageRequest(characterBody.masterObjectId, Target.healthComponent.body.masterObjectId, damageStat * distance).Send(NetworkDestination.Clients);
                        keepMoving = false;
                    }
                }
                else
                {
                    this.storedPosition = direction;
                    if (base.isAuthority)
                    {
                        Vector3 velocity = (this.storedPosition - base.transform.position).normalized * dashSpeed;
                        base.characterMotor.velocity = velocity;
                        base.characterDirection.forward = base.characterMotor.velocity.normalized;
                    }

                }
            }

            if(base.fixedAge > duration * warpEndTime)
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
                }

            }
            
            if(base.fixedAge > duration)
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



