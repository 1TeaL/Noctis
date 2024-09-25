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
    public class Dodge : BaseSkillState
    {
        private Animator animator;
        private CharacterModel characterModel;
        public NoctisController noctisCon;
        public EnergySystem energySystem;
        private Ray aimRay;
        private float rollSpeed;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.dodgeSpeed;
        private float finalSpeedCoefficient = 0.1f;

        private float baseDuration = 0.6f;
        private float duration;

        

        private Vector3 direction;
        private Transform modelTransform;

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

            if (!characterBody.HasBuff(Buffs.armigerBuff))
            {
                if (energySystem.currentMana < manaCost)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
                else if (energySystem.currentMana >= manaCost)
                {
                    energySystem.SpendMana(manaCost);
                }
            }

            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();

                TemporaryOverlayInstance temporaryOverlay = TemporaryOverlayManager.AddOverlay(new GameObject());
                temporaryOverlay.duration = 0.3f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                TemporaryOverlayInstance temporaryOverlay2 = TemporaryOverlayManager.AddOverlay(new GameObject());
                temporaryOverlay2.duration = 0.3f;
                temporaryOverlay2.animateShaderAlpha = true;
                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay2.destroyComponentOnEnd = true;
                temporaryOverlay2.originalMaterial = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");

            }

            this.animator = base.GetModelAnimator();
            aimRay = base.GetAimRay();
            //noctisCon.WeaponAppearR(0f, NoctisController.WeaponTypeR.NONE);

            if(noctisCon.weaponStateR != NoctisController.WeaponTypeR.NONE)
            {

                EffectManager.SimpleMuzzleFlash(Modules.NoctisAssets.noctisSwingEffect, base.gameObject, "SwordSwingStab", true);

                BlastAttack blastAttack = new BlastAttack();

                blastAttack.position = base.characterBody.corePosition;
                blastAttack.baseDamage = this.damageStat * 1f;
                blastAttack.baseForce = 0f;
                blastAttack.radius = 4f;
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = base.teamComponent.teamIndex;
                blastAttack.crit = base.RollCrit();
                blastAttack.procChainMask = default(ProcChainMask);
                blastAttack.procCoefficient = 0.1f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.damageColorIndex = DamageColorIndex.Default;
                blastAttack.damageType = DamageType.Generic;
                blastAttack.attackerFiltering = AttackerFiltering.Default;


                int attackAmount = (int)this.attackSpeedStat;
                if (attackAmount < 1)
                {
                    attackAmount = 1;
                }
                float partialAttack = (float)(this.attackSpeedStat - (float)attackAmount);

                for (int i = 0; i < attackAmount; i++)
                {
                    blastAttack.Fire();
                }
                if (partialAttack > 0f)
                {
                    blastAttack.baseDamage = this.damageStat * partialAttack;
                    blastAttack.procCoefficient = 0.1f * partialAttack;
                    blastAttack.Fire();
                }
            }

            direction = base.inputBank.moveVector;
            duration = baseDuration / attackSpeedStat;

            base.GetModelAnimator().SetFloat("Attack.playbackRate", attackSpeedStat);
            SpeedCoefficient = initialSpeedCoefficient * attackSpeedStat;

            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 1);
            PlayAnimation();
        }

        private void PlayAnimation()
        {
            AkSoundEngine.PostEvent("Dodge", base.gameObject);
            if (!characterMotor.isGrounded)
            {
                //base.PlayCrossfade("FullBody, Override", "AerialDodge", "Attack.playbackRate", this.duration, 0.05f);
                base.PlayAnimation("FullBody, Override", "AerialDodge", "Attack.playbackRate", this.duration);
                
            }
            else
            if (characterMotor.isGrounded)
            {
                //base.PlayCrossfade("FullBody, Override", "Roll", "Attack.playbackRate", this.duration, 0.05f);
                base.PlayAnimation("FullBody, Override", "Dash", "Attack.playbackRate", this.duration);
               
            }
        }

        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            this.rollSpeed = num * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 0);
        }

        public override void Update()
        {
            base.Update();
            RecalculateRollSpeed();
            base.characterMotor.velocity = Vector3.zero;
                    

            if (base.fixedAge <= duration)
            {
                //if (!characterMotor.isGrounded)
                //{
                //    base.SmallHop(base.characterMotor, StaticValues.dodgeHop);
                //}
                if (!characterMotor.isGrounded)
                {
                    base.StartAimMode(0.5f, true);
                    base.characterMotor.rootMotion += this.direction * this.rollSpeed * Time.fixedDeltaTime;
                    base.SmallHop(base.characterMotor, StaticValues.dodgeHop);
                }
                else
                if (characterMotor.isGrounded)
                {
                    base.characterDirection.forward = this.direction;
                    base.characterMotor.rootMotion += this.direction * this.rollSpeed * Time.fixedDeltaTime;
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



