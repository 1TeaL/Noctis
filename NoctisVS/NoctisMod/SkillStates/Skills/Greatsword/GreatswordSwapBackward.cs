﻿using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using NoctisMod.Modules;
using EntityStates.Huntress;
using static NoctisMod.Modules.Survivors.NoctisController;
using R2API.Networking;

namespace NoctisMod.SkillStates
{
    public class GreatswordSwapBackward : BaseSkillState
    {
        private Transform modelTransform;
        private CharacterModel characterModel;
        public NoctisController noctisCon;
        float baseDuration = 0.7f;
        private Animator animator;
        private float chargeMultiplier;
        private float chargePercent;
        private float maxCharge = StaticValues.GSMaxCharge;
        private float damageMult;
        private float baseDistance = 2f;
        private RaycastHit raycastHit;
        private float hitDis;
        private GameObject areaIndicator;
        private float radius;
        private float baseRadius = StaticValues.GSChargeRadius;
        private Vector3 maxMoveVec;

        public override void OnEnter()
        {
            base.OnEnter();
            noctisCon = GetComponent<NoctisController>();
            this.animator = base.GetModelAnimator();
            base.StartAimMode(this.baseDuration, false);
            this.animator.SetBool("releaseChargeSlash", false);
            this.animator.SetBool("releaseChargeLeap", false);
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);

            base.PlayCrossfade("FullBody, Override", "GSCharge", "Attack.playbackRate", this.baseDuration, 0.05f);
            this.areaIndicator = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
            this.areaIndicator.SetActive(true);

            characterBody.ApplyBuff(Modules.Buffs.GSarmorBuff.buffIndex, 1);

            chargeMultiplier = 1f;
            if (noctisCon.isSwapped)
            {
                chargeMultiplier = 3f;
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
            }
            else
            {
                chargeMultiplier = 1f;
            }
        }

        public void ChargeCalc()
        {
            this.chargePercent = base.fixedAge * attackSpeedStat / this.maxCharge;
            Ray aimRay = base.GetAimRay();
            Vector3 direction = aimRay.direction;
            aimRay.origin = base.characterBody.corePosition;
            Physics.Raycast(aimRay.origin, aimRay.direction, out this.raycastHit, this.baseDistance);
            this.hitDis = this.raycastHit.distance;
            bool flag = this.hitDis < this.baseDistance && this.hitDis > 0f;
            if (flag)
            {
                this.baseDistance = this.hitDis;
            }
            this.damageMult = StaticValues.GSChargeDamage + StaticValues.GSChargeMultiplier * (this.chargePercent * StaticValues.GSChargeDamage);
            this.radius = (this.baseRadius * this.damageMult + 10f) / 4f;
            this.maxMoveVec = this.baseDistance * direction;
            this.areaIndicator.transform.localScale = Vector3.one * this.radius;
            this.areaIndicator.transform.localPosition = aimRay.origin + this.maxMoveVec;
            noctisCon.WeaponAppearR(3f, WeaponTypeR.GREATSWORD);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterMotor.velocity = Vector3.zero;
            if (base.fixedAge < maxCharge)
            {
                if (inputBank.skill1.down && skillLocator.primary.skillDef == Noctis.greatswordSkillDef)
                {
                    ChargeCalc();

                }
                else if (inputBank.skill2.down && skillLocator.secondary.skillDef == Noctis.greatswordSkillDef)
                {
                    ChargeCalc();


                }
                else if (inputBank.skill4.down && skillLocator.special.skillDef == Noctis.greatswordSkillDef)
                {
                    ChargeCalc();

                }
                else
                {
                    GreatswordSwapBackward2 GreatswordSwapBackward2 = new GreatswordSwapBackward2();
                    GreatswordSwapBackward2.damageMult = damageMult;
                    GreatswordSwapBackward2.radius = this.radius;
                    this.outer.SetNextState(GreatswordSwapBackward2);
                    this.animator.SetBool("releaseChargeLeap", true);
                    return;
                }
            }
            else
            {
                GreatswordSwapBackward2 GreatswordSwapBackward2 = new GreatswordSwapBackward2();
                GreatswordSwapBackward2.damageMult = damageMult;
                GreatswordSwapBackward2.radius = this.radius;
                this.outer.SetNextState(GreatswordSwapBackward2);
                this.animator.SetBool("releaseChargeLeap", true);
                return;
            }

        }


        public override void OnExit()
        {
            base.OnExit();
            if (this.areaIndicator)
                this.areaIndicator.SetActive(false);
            EntityState.Destroy(this.areaIndicator);
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.damageMult);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.damageMult = reader.ReadInt64();
        }
    }
}



