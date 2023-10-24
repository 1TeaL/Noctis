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

namespace NoctisMod.SkillStates
{
    public class SwordSwapNeutral : BaseMeleeAttack
    {
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("ShiggyMelee", base.gameObject);

            weaponDef = Noctis.swordSkillDef;
            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = 1f;
            this.procCoefficient = 1f;
            this.pushForce = 0f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.4f;
            this.attackEndTime = 1f;
            this.baseEarlyExitTime = 1f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "ShiggyMelee";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSwingUp";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1);

            base.OnEnter();
            autoStateChange = true;

        }



        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "SwordUpDownSlash", "Attack.playbackRate", 2f, 0.05f);
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



