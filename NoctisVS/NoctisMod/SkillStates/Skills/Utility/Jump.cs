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

namespace NoctisMod.SkillStates
{
    public class Jump : BaseSkillState
    {
        private Animator animator;
        private CharacterModel characterModel;
        public NoctisController noctisCon;
        public EnergySystem energySystem;
        private Ray aimRay;
        private float rollSpeed;
        private float SpeedCoefficient;
        private Transform modelTransform;
        public static float initialSpeedCoefficient = Modules.StaticValues.jumpSpeed;
        private float finalSpeedCoefficient = 0.1f;

        private float baseDuration = 0.5f;
        private float earlyExitTime = 0.6f;
        private float duration;

        private Vector3 direction;


        public override void OnEnter()
        {

            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            energySystem = gameObject.GetComponent<EnergySystem>();

            //energy cost
            float manaflatCost = (StaticValues.dodgeCost) - (energySystem.costflatMana);
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

            this.animator = base.GetModelAnimator();
            aimRay = base.GetAimRay();

            direction = base.inputBank.moveVector;
            duration = baseDuration;

            base.GetModelAnimator().SetFloat("Attack.playbackRate", attackSpeedStat);
            SpeedCoefficient = initialSpeedCoefficient;

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
            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 1);
            PlayAnimation();
        }

        private void PlayAnimation()
        {
            base.PlayAnimation("FullBody, Override", "AerialDodge", "Attack.playbackRate", this.duration);
            AkSoundEngine.PostEvent("Dodge", base.gameObject);
        }

        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            this.rollSpeed = num * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / duration * earlyExitTime);
        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 0);
        }

        public override void Update()
        {
            base.Update();
                    

            if (base.fixedAge <= duration * earlyExitTime)
            {
                RecalculateRollSpeed();
                base.characterMotor.velocity = Vector3.zero;
                base.StartAimMode(0.5f, true);
                base.characterMotor.rootMotion += this.direction * this.rollSpeed * Time.fixedDeltaTime;
                base.characterDirection.forward = base.characterMotor.velocity.normalized;
                base.SmallHop(base.characterMotor, StaticValues.jumpHop);
            }

            if (base.fixedAge > duration * earlyExitTime)
            {
                if (inputBank.skill1.down)
                {
                    if(skillLocator.primary.skillDef == Noctis.polearmSkillDef && inputBank.jump.down)
                    {
                        this.outer.SetNextState(new PolearmDoubleDragoonThrust());
                        return;
                    }
                    else
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                if (inputBank.skill2.down)
                {
                    if (skillLocator.secondary.skillDef == Noctis.polearmSkillDef && inputBank.jump.down)
                    {
                        this.outer.SetNextState(new PolearmDoubleDragoonThrust());
                        return;
                    }
                    else
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                if (inputBank.skill3.down)
                {
                    this.outer.SetNextState(new Dodge());
                    return;
                }
                if (inputBank.skill4.down)
                {
                    if (skillLocator.special.skillDef == Noctis.polearmSkillDef && inputBank.jump.down)
                    {
                        this.outer.SetNextState(new PolearmDoubleDragoonThrust());
                        return;
                    }
                    else
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }


            if (base.fixedAge > duration)
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



