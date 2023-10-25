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

            this.damageType = DamageType.Generic;

            this.damageCoefficient = StaticValues.GSDamage;
            this.procCoefficient = StaticValues.GSProc;
            this.pushForce = 300f;
            this.baseDuration = 2.7f;
            this.attackStartTime = 0.1f;
            this.attackEndTime = 0.8f;
            this.baseEarlyExitTime = 0.8f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "GreatswordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingDown";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            base.OnEnter();
            if (base.isAuthority)
            {
                if (Modules.Config.allowVoice.Value) { AkSoundEngine.PostEvent("NoctisVoice", base.gameObject); }
            }

        }



        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "GSOverheadSlash", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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



