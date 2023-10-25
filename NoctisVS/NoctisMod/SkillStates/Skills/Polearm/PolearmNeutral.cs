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
    public class PolearmNeutral : BaseMeleeAttack
    {
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.polearmSkillDef;
            this.hitboxName = "PolearmThrustHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = StaticValues.polearmDamage;
            this.procCoefficient = StaticValues.polearmProc;
            this.pushForce = 500f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1f;
            this.attackStartTime = 0.4f;
            this.attackEndTime = 0.8f;
            this.baseEarlyExitTime = 0.8f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "PolearmSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingStab";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            base.OnEnter();
            attackAmount += StaticValues.polearmExtraHit;
          
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "PolearmNeutralThrust", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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



