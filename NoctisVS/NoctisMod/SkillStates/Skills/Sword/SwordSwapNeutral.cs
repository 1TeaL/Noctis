﻿using EntityStates;
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
    public class SwordSwapNeutral : BaseMeleeAttack
    {
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);

            weaponDef = Noctis.swordSkillDef;
            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = StaticValues.swordDamage;
            this.procCoefficient = StaticValues.swordProc;
            this.pushForce = 0f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.6f;
            this.attackEndTime = 0.8f;
            this.baseEarlyExitTime = 0.8f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;


            this.swingSoundString = "SwordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSwingUp";
            this.swingEffectPrefab = Modules.NoctisAssets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.NoctisAssets.noctisHitEffect;

            this.impactSound = Modules.NoctisAssets.hitSoundEffect.index;

            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1);

            base.OnEnter();
            autoStateChange = true;
            if (isSwapped)
            {
                this.baseDuration = 0.4f;
                this.attackStartTime = 0.01f;
                this.attackEndTime = 0.5f;
                this.baseEarlyExitTime = 0.5f;
            }

        }



        protected override void PlayAttackAnimation()
        {
            if (isSwapped)
            {
                animator.Play("FullBody, Override.SwordUpDownSlashPart1", -1, 0.6f);
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "SwordUpDownSlashPart1", "Attack.playbackRate", baseDuration - baseEarlyExitTime, 0.05f);
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
            if (!this.hasFired) this.FireAttack();

            this.outer.SetNextState(new SwordSwapNeutral2());
            return;          


        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0);

        }

    }
}



