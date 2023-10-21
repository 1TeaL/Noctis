using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;

namespace NoctisMod.SkillStates
{
    public class SwordNeutral : BaseMeleeAttack
    {
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("ShiggyMelee", base.gameObject);

            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;
            switch (this.swingIndex)
            {
                case 0:
                    this.damageCoefficient = 1f;
                    this.procCoefficient = 1f;
                    this.pushForce = 300f;
                    this.baseDuration = 0.5f;
                    this.attackStartTime = 0.2f;
                    this.attackEndTime = 0.4f;
                    this.baseEarlyExitTime = 0.4f;
                    break;
                case 1:
                    this.damageCoefficient = 1f;
                    this.procCoefficient = 1f;
                    this.pushForce = 0f;
                    this.baseDuration = 0.5f;
                    this.attackStartTime = 0.2f;
                    this.attackEndTime = 0.4f;
                    this.baseEarlyExitTime = 0.4f;
                    break;
                case 2:
                    this.damageCoefficient = 1f;
                    this.procCoefficient = 1f;
                    this.pushForce = 1000f;
                    this.baseDuration = 1f;
                    this.attackStartTime = 0.4f;
                    this.attackEndTime = 0.8f;
                    this.baseEarlyExitTime = 0.4f;
                    break;
            }
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "ShiggyMelee";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSwingNeutral{this.swingIndex + 1}";
            this.swingEffectPrefab = Modules.Assets.noctisHitEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            base.OnEnter();

        }



        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "SwordSlashNeutral" + (1 + swingIndex), "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();

        }

        protected override void CheckNextState()
        {
            if (!this.hasFired) this.FireAttack();

            if (base.isAuthority && base.IsKeyDownAuthority())
            {
                int index = this.swingIndex;
                index += 1;
                if (index > 2)
                {
                    index = 0;
                }
                this.outer.SetNextState(new SwordCombo
                {
                    currentSwingIndex = index
                });

                return;

            }


        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}



