using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using NoctisMod.Modules;

namespace NoctisMod.SkillStates
{
    public class GreatswordSwapForward : BaseMeleeAttack
    {
        public override void OnEnter()
        {

            weaponDef = Noctis.greatswordSkillDef;
            this.hitboxName = "GreatswordHitbox";

            this.damageType = DamageType.Stun1s;

            this.damageCoefficient = StaticValues.GSSwapForwardDamage;
            this.procCoefficient = StaticValues.GSProc;
            this.pushForce = 300f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.33f;
            this.attackEndTime = 0.68f;
            this.baseEarlyExitTime = 0.68f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "GreatswordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingDown";
            this.swingEffectPrefab = Modules.NoctisAssets.noctisSwingEffectMedium;
            this.hitEffectPrefab = Modules.NoctisAssets.noctisHitEffect;

            this.impactSound = Modules.NoctisAssets.hitSoundEffect.index;

            base.OnEnter();
            hasVulnerability = true;
            if (isSwapped)
            {
                this.baseDuration = 0.67f;
                this.attackStartTime = 0.01f;
                this.attackEndTime = 0.5f;
                this.baseEarlyExitTime = 0.5f;
            }

        }



        protected override void PlayAttackAnimation()
        {

            if (isSwapped)
            {
                animator.Play("FullBody, Override.GSOverheadSlash", -1, 0.33f);
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "GSOverheadSlash", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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
            if (base.isAuthority)
            {
                if (!this.hasFired) this.FireAttack();
                this.outer.SetNextState(new GreatswordCombo());
                return;

            }


        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}



