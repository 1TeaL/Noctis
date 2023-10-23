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
            weaponDef = Noctis.swordSkillDef;
            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;

            if (swingIndex == 0)
            {
                this.damageCoefficient = 2f;
                this.procCoefficient = 1f;
                this.pushForce = 300f;
                this.baseDuration = 1f;
                this.attackStartTime = 0.3f;
                this.attackEndTime = 0.6f;
                this.baseEarlyExitTime = 0.4f;
            }
            else if (swingIndex == 1)
            {
                this.damageCoefficient = 1f;
                this.procCoefficient = 1f;
                this.pushForce = 0f;
                this.baseDuration = 1f;
                this.attackStartTime = 0.3f;
                this.attackEndTime = 0.6f;
                this.baseEarlyExitTime = 0.4f;
            }
            else if (swingIndex == 2)
            {
                this.damageCoefficient = 4f;
                this.procCoefficient = 1f;
                this.pushForce = 1000f;
                this.baseDuration = 2f;
                this.attackStartTime = 0.3f;
                this.attackEndTime = 0.6f;
                this.baseEarlyExitTime = 1f;
            }

            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "ShiggyMelee";
            this.hitSoundString = "";
            this.muzzleString = ChooseMuzzleString();
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            base.OnEnter();

        }

        private string ChooseMuzzleString()
        {
            string returnVal = "SwordSwingDown";
            switch (this.swingIndex)
            {
                case 0:
                    returnVal = "SwordSwingDown";
                    break;
                case 1:
                    returnVal = "SwordSwingStab";
                    break;
                case 2:
                    returnVal = "SwordSwingRight";
                    break;
            }

            return returnVal;
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

        protected override void SetNextState()
        {
            if (base.isAuthority)
            {
                if (!this.hasFired) this.FireAttack();
                int index = this.swingIndex;
                index += 1;
                if (index > 2)
                {
                    index = 0;
                }
                SwordCombo SwordCombo = new SwordCombo();
                SwordCombo.currentSwingIndex = index;
                this.outer.SetNextState(SwordCombo);
                return;

            }


        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}



