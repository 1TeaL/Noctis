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
using System.Linq;

namespace NoctisMod.SkillStates
{
    public class Warpstrike : BaseSkillState
    {
        private Animator animator;
        private CharacterModel characterModel;
        public NoctisController noctisCon;
        public EnergySystem energySystem;
        private Ray aimRay;
        public float dashSpeed = Modules.StaticValues.warpstrikeSpeed;

        private float duration = 1.5f;
        private float warpStartTime = 0.16f;
        private float warpEndTime = 0.6f;
        private bool keepMoving;

        private Vector3 direction;
        private bool isTarget;
        private HurtBox Target;
        private float distance;
        private Vector3 storedPosition;
        private Transform modelTransform;

        private bool hasFired;
        private bool isFreeze;
        public bool weaponSwap;

        public override void OnEnter()
        {

            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            energySystem = gameObject.GetComponent<EnergySystem>();
            this.animator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            animator.SetFloat("Attack.playbackRate", 2f);
            animator.SetBool("attacking", true);
            base.StartAimMode(this.duration, false);

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

            if (noctisCon.GetTrackingTarget())
            {
                Target = noctisCon.GetTrackingTarget();
                distance = Vector3.Magnitude(Target.transform.position - base.characterBody.corePosition);
                isTarget = true;
                this.storedPosition = Target.transform.position;   
                
                if(distance >= StaticValues.maxTrackingDistance * StaticValues.warpstrikeThreshold)
                {
                    isFreeze = true;
                }
            }



            noctisCon.WeaponAppearR(0f, NoctisController.WeaponTypeR.NONE);
            noctisCon.WeaponAppearL(0f, NoctisController.WeaponTypeL.NONE);
            noctisCon.DashParticle.Play();

            aimRay = base.GetAimRay();
            keepMoving = true;

            this.direction = aimRay.direction.normalized;
            base.characterDirection.forward = direction;


            PlayAnimation();
            EffectManager.SpawnEffect(Assets.swordThrowParticle, new EffectData
            {
                origin = aimRay.origin,
                rotation = Quaternion.LookRotation(new Vector3(aimRay.direction.x, aimRay.direction.y, aimRay.direction.z)),
            }, true);
            EffectManager.SpawnEffect(Assets.noctisDashEffect, new EffectData
            {
                origin = characterBody.corePosition,
                rotation = Quaternion.LookRotation(new Vector3(aimRay.direction.x, aimRay.direction.y, aimRay.direction.z)),
            }, true);
        }

        private void PlayAnimation()
        {
            AkSoundEngine.PostEvent("Warpstrike", base.gameObject);

            if (weaponSwap)
            {
                characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1, 2);

                if (isTarget)
                {
                    animator.Play("FullBody, Override.WarpStrike", -1, 0.25f);
                    duration = 1f;
                    warpStartTime = 0.05f;
                    warpEndTime = 0.6f;
                }
                else
                {
                    animator.Play("FullBody, Override.WarpStrikeRoll", -1, 0.15f);
                    duration = 1f;
                    warpStartTime = 0.05f;
                    warpEndTime = 0.6f;
                }
            }
            else
            {
                characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1, 2);

                if (isTarget)
                {
                    base.PlayAnimation("FullBody, Override", "WarpStrike", "Attack.playbackRate", this.duration);
                }
                else
                {
                    base.PlayAnimation("FullBody, Override", "WarpStrikeRoll", "Attack.playbackRate", this.duration);
                }
            }
        }

        public void Freeze()
        {
            BullseyeSearch search = new BullseyeSearch
            {

                teamMaskFilter = TeamMask.GetEnemyTeams(characterBody.teamComponent.teamIndex),
                filterByLoS = false,
                searchOrigin = characterBody.corePosition,
                searchDirection = UnityEngine.Random.onUnitSphere,
                sortMode = BullseyeSearch.SortMode.Distance,
                maxDistanceFilter = StaticValues.warpstrikeFreezeRange,
                maxAngleFilter = 360f
            };

            search.RefreshCandidates();
            search.FilterOutGameObject(characterBody.gameObject);

            List<HurtBox> target = search.GetResults().ToList<HurtBox>();
            foreach (HurtBox singularTarget in target)
            {
                if (singularTarget.healthComponent && singularTarget.healthComponent.body)
                {
                    //stop time for all enemies within this radius

                    //Chat.AddMessage("freeze enemy");
                    new SetFreezeOnBodyRequest(singularTarget.healthComponent.body.masterObjectId).Send(NetworkDestination.Clients);


                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            noctisCon.DashParticle.Stop();
            noctisCon.WeaponAppearR(0f, NoctisController.WeaponTypeR.NONE);
            animator.SetBool("attacking", false);
        }

        public override void Update()
        {
            base.Update();                   

            if (base.fixedAge >= duration * warpStartTime && base.fixedAge <= duration * warpEndTime && keepMoving)
            {
                if (this.modelTransform)
                {
                    this.animator = this.modelTransform.GetComponent<Animator>();
                    this.characterModel = this.modelTransform.GetComponent<CharacterModel>();

                    TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 0.3f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                    TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay2.duration = 0.3f;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                    temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());

                }

                base.characterDirection.forward = direction;
                if (isTarget)
                {
                    this.storedPosition = Target.transform.position;
                    if(base.isAuthority)
                    {
                        //Vector3 velocity = (this.storedPosition - base.transform.position).normalized * dashSpeed;
                        //base.characterMotor.velocity = velocity;
                        //base.characterMotor.velocity = Vector3.zero;
                        //base.characterMotor.rootMotion += (this.storedPosition - base.transform.position).normalized * this.dashSpeed * Time.deltaTime;

                        Vector3 velocity = (this.storedPosition - base.transform.position).normalized * dashSpeed;
                        base.characterMotor.velocity = velocity;
                        base.characterDirection.forward = base.characterMotor.velocity.normalized;
                    }

                    if (Vector3.Magnitude(this.storedPosition - base.transform.position) <= 10f && !hasFired)
                    {
                        hasFired = true;

                        if (isFreeze)
                        {
                            Freeze();
                        }

                        new TakeDamageRequest(characterBody.masterObjectId, Target.healthComponent.body.masterObjectId, damageStat * distance * StaticValues.warpstrikeDamageScaling, characterDirection.forward, false, false).Send(NetworkDestination.Clients);
                        keepMoving = false;
                        base.PlayAnimation("FullBody, Override", "WarpStrikeAttack", "Attack.playbackRate", 0.01f);
                        AkSoundEngine.PostEvent("NoctisHitSFX", base.gameObject);
                        noctisCon.WeaponAppearR(1f, NoctisController.WeaponTypeR.SWORD);

                    }
                }
                else
                {
                    if (base.isAuthority)
                    {
                        //base.characterDirection.forward = this.direction;
                        //base.characterMotor.velocity = Vector3.zero;
                        //base.characterMotor.rootMotion += this.direction * this.dashSpeed * Time.deltaTime;

                        Vector3 velocity = direction.normalized * dashSpeed;
                        base.characterMotor.velocity = velocity;
                        base.characterDirection.forward = base.characterMotor.velocity.normalized;
                    }

                }
            }


            if(base.fixedAge > duration * warpEndTime || !keepMoving)
            {
                base.characterMotor.velocity *= 0.1f;
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
            return InterruptPriority.Skill;
        }

    }
}



