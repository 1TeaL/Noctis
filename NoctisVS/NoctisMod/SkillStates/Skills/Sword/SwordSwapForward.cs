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
using R2API.Utils;
using System.Linq;
using EntityStates.Merc;
using NoctisMod.Modules.Networking;
using R2API.Networking.Interfaces;
using IL.RoR2.Achievements.Railgunner;

namespace NoctisMod.SkillStates
{
    [R2APISubmoduleDependency(new string[]
    {
        "NetworkingAPI"
    })]
    public class SwordSwapForward : BaseSkillState
    {

        public float previousMass;
        private string muzzleString;

        public int numberOfHits;
        public static float baseDuration = 3f;
        public static float startMoving = 0.33f;
        public static float endMoving = 0.46f;
        public static float earlyExitTime = 0.7f;
        public static float initialSpeedCoefficient = StaticValues.swordInstaDashSpeed;
        public static float finalSpeedCoefficient = 0f;
        public static float SpeedCoefficient;
        public static float procCoefficient = StaticValues.swordProc;
        private Animator animator;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private float rollSpeed;
        private Vector3 direction;

        //checking location for networking
        public Vector3 origin;
        public Vector3 final;
        private Vector3 theSpot;
        private readonly BullseyeSearch search = new BullseyeSearch();

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();

            this.RecalculateRollSpeed();

            this.direction = base.GetAimRay().direction.normalized;
            this.direction.y = 0f;

            numberOfHits = Mathf.RoundToInt(StaticValues.swordBaseHit + attackSpeedStat);


            this.animator.SetBool("attacking", true);
            base.PlayCrossfade("FullBody, Override", "SwordInstaSlash", "Attack.playbackRate", baseDuration, 0.05f);

            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1);

            base.characterMotor.useGravity = false;
            this.previousMass = base.characterMotor.mass;
            base.characterMotor.mass = 0f;

            SpeedCoefficient = initialSpeedCoefficient;
            origin = base.transform.position;
        }
        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            if(num > 20f)
            {
                num = 20f;
            }
            this.rollSpeed = num * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / baseDuration);
        }

        public void DealDamage()
        {
            theSpot = Vector3.Lerp(origin, final, 0.5f);

            search.teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam());
            search.filterByLoS = false;
            search.searchOrigin = theSpot;
            search.searchDirection = UnityEngine.Random.onUnitSphere;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = (final - origin).magnitude / 2;
            search.maxAngleFilter = 360f;


            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);



            List<HurtBox> target = search.GetResults().ToList<HurtBox>();
            foreach (HurtBox singularTarget in target)
            {
                if (singularTarget.healthComponent.body && singularTarget.healthComponent)
                {
                    for (int i = 0; i <= numberOfHits; i += 1)
                    {
                        new TakeDamageRequest(characterBody.masterObjectId, singularTarget.healthComponent.body.masterObjectId, damageStat * StaticValues.swordSwapForwardDamage).Send(NetworkDestination.Clients);
                    }

                }
            }
        }

        public override void OnExit()
        {
            Ray aimRay = base.GetAimRay();
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0);
            this.animator.SetBool("attacking", false);
            Util.PlaySound(EvisDash.endSoundString, base.gameObject);

            base.characterMotor.mass = this.previousMass;
            base.characterMotor.useGravity = true;
            base.characterMotor.velocity = Vector3.zero;

            if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = -1f;
            base.characterMotor.disableAirControlUntilCollision = false;
            base.characterMotor.velocity.y = 0;

            final = base.transform.position;
            DealDamage();

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge == baseDuration * endMoving)
            {
                EffectManager.SimpleMuzzleFlash(Assets.noctisSwingEffect, base.gameObject, "SwordSwingRight", true);
            }

            if(base.fixedAge >= baseDuration * startMoving && base.fixedAge < baseDuration * endMoving)
            {
                this.RecalculateRollSpeed();
                Vector3 velocity = this.direction * rollSpeed;
                velocity.y = base.characterMotor.velocity.y;
                base.characterMotor.velocity = velocity;
                base.characterDirection.forward = base.characterMotor.velocity.normalized;

                if (this.modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 0.6f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                    TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay2.duration = 0.7f;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                    temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                }

            }
            if(base.fixedAge > baseDuration * earlyExitTime)
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

            if (base.isAuthority && base.fixedAge >= baseDuration)
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



