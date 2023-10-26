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
    public class PolearmSwapNeutral : BaseMeleeAttack
    {
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.polearmSkillDef;
            this.hitboxName = "GreatswordHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = StaticValues.polearmDamage;
            this.procCoefficient = StaticValues.polearmProc;
            this.pushForce = 1000f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 2f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.5f;
            this.baseEarlyExitTime = 0.5f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "PolearmSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingLeft";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            base.OnEnter();
            attackAmount += StaticValues.polearmSwapExtraHit;
          

        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "PolearmSweep", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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

                PolearmCombo PolearmCombo = new PolearmCombo();
                this.outer.SetNextState(PolearmCombo);
                return;
            }

        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}



