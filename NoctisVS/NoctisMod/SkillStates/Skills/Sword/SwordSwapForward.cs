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
        NoctisController noctisCon;
        public bool isSwapped;
        public float previousMass;
        private string muzzleString;

        public int numberOfHits;
        private float partialAttack;
        public float baseDuration = 3f;
        public float startMoving = 0.33f;
        public float endMoving = 0.46f;
        public float earlyExitTime = 0.5f;
        public static float initialSpeedCoefficient = StaticValues.swordInstaDashSpeed;
        public static float finalSpeedCoefficient = 0f;
        public static float SpeedCoefficient;
        public static float procCoefficient = StaticValues.swordProc;
        private Animator animator;

        private HurtBoxGroup hurtboxGroup;
        private Vector3 forwardDirection;
        private Transform modelTransform;
        private CharacterModel characterModel;
        private float rollSpeed;
        private Vector3 direction;
        private bool playSwing;

        //checking location for networking
        public Vector3 origin;
        public Vector3 final;
        private Vector3 theSpot;
        private readonly BullseyeSearch search = new BullseyeSearch();

        public override void OnEnter()
        {
            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            isSwapped = noctisCon.isSwapped;
            this.animator = base.GetModelAnimator();

            this.RecalculateRollSpeed();

            this.direction = base.GetAimRay().direction.normalized;
            this.direction.y = 0f;

            numberOfHits = Mathf.RoundToInt(StaticValues.swordBaseHit + attackSpeedStat);

            numberOfHits = (int)this.attackSpeedStat + StaticValues.swordBaseHit;
            partialAttack = (float)(this.attackSpeedStat - (float)numberOfHits);


            this.animator.SetBool("attacking", true);
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 2f);

            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1);

            //this.previousMass = base.characterMotor.mass;
            //base.characterMotor.mass = 0f;

            SpeedCoefficient = initialSpeedCoefficient;
            origin = base.transform.position;
            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();


            if (isSwapped)
            {
                animator.Play("FullBody, Override.SwordInstaSlash", -1, 0.22f);
                this.baseDuration = 2.3f;
                this.startMoving = 0.01f;
                this.endMoving = 0.17f;
                this.earlyExitTime = 0.2f;

                this.modelTransform = base.GetModelTransform();
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
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "SwordInstaSlash", "Attack.playbackRate", baseDuration, 0.05f);
            }

            if (base.inputBank && base.characterDirection)
            {
                base.characterDirection.forward = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            }
            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            Util.PlaySound(Assaulter2.endSoundString, base.gameObject);

            if (base.isAuthority)
            {
                AkSoundEngine.PostEvent("NoctisVoice", this.gameObject);
            }

            noctisCon.SetSwapTrue(baseDuration);

        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.forwardDirection);
            effectData.origin = origin;
            EffectManager.SpawnEffect(EvisDash.blinkPrefab, effectData, false);
        }
        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            float num2 = (num / base.characterBody.baseMoveSpeed - 1f) * 0.67f;
            float num3 = num2 + 1f;
            this.rollSpeed = num3 * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / baseDuration);
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
                        new TakeDamageRequest(characterBody.masterObjectId, singularTarget.healthComponent.body.masterObjectId, damageStat * StaticValues.swordSwapForwardDamage, characterBody.characterDirection.forward, false, false).Send(NetworkDestination.Clients);
                    }
                    if(partialAttack > 0f)
                    {
                        new TakeDamageRequest(characterBody.masterObjectId, singularTarget.healthComponent.body.masterObjectId, damageStat * StaticValues.swordSwapForwardDamage * partialAttack, characterBody.characterDirection.forward, false, false).Send(NetworkDestination.Clients);

                    }

                }
            }
        }

        public override void OnExit()
        {
            Ray aimRay = base.GetAimRay();
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0);
            this.animator.SetBool("attacking", false);

            //base.characterMotor.mass = this.previousMass;
            //base.characterMotor.useGravity = true;
            //base.characterMotor.velocity = Vector3.zero;

            if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = -1f;
            base.characterMotor.disableAirControlUntilCollision = false;
            base.characterMotor.velocity.y = 0;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            if (this.characterModel)
            {
                this.characterModel.invisibilityCount--;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

            if (characterBody.HasBuff(Buffs.GSarmorBuff))
            {
                characterBody.ApplyBuff(Buffs.GSarmorBuff.buffIndex, 0);
            }
            if (characterBody.HasBuff(Buffs.armorBuff))
            {
                characterBody.ApplyBuff(Buffs.armorBuff.buffIndex, 0);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge > baseDuration * endMoving)
            {
                base.characterMotor.velocity = Vector3.zero;
                if (!playSwing)
                {

                    if (this.characterModel)
                    {
                        this.characterModel.invisibilityCount--;
                    }
                    if (this.hurtboxGroup)
                    {
                        HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                        int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                        hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                    }
                    playSwing = true;
                    EffectManager.SimpleMuzzleFlash(Assets.noctisSwingEffect, base.gameObject, "SwordSwingRight", true);
                    Util.PlaySound(EvisDash.endSoundString, base.gameObject);
                    final = base.transform.position;
                    DealDamage();
                }
            }

            if(base.fixedAge >= baseDuration * startMoving && base.fixedAge < baseDuration * endMoving)
            {
                this.RecalculateRollSpeed();
                //Vector3 velocity = this.direction * rollSpeed;
                //velocity.y = base.characterMotor.velocity.y;
                //velocity.y = 0f;
                //base.characterMotor.velocity = velocity;
                //base.characterDirection.forward = base.characterMotor.velocity.normalized;

                if (base.inputBank && base.characterDirection)
                {
                    base.characterDirection.moveVector = base.inputBank.moveVector;
                    this.forwardDirection = base.characterDirection.forward;
                }
                if (base.characterMotor)
                {
                    
                    base.characterMotor.rootMotion += rollSpeed * this.forwardDirection * Time.fixedDeltaTime;
                }

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



