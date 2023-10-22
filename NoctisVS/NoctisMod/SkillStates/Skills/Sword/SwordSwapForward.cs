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

namespace NoctisMod.SkillStates
{
    public class SwordSwapForward : BaseMeleeAttack
    {
        public HurtBox Target;
        public bool isTarget;
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
            this.attackEndTime = 0.8f;
            this.baseEarlyExitTime = 0.4f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "ShiggyMelee";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSwingRight";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1, 0);

            base.OnEnter();

        }



        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "SwordInstaSlash", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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

                this.outer.SetNextState(new SwordCombo());
                return;

            }


        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0, 0);

        }

    }
}



