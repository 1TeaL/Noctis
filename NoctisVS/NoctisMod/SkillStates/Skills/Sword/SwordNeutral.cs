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

            this.damageCoefficient = 1f;
            this.procCoefficient = 1f;
            this.pushForce = 0f;
            this.damageType = DamageType.Generic;
            this.baseDuration = 1f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.4f;
            this.hitStopDuration = 0.08f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "ShiggyMelee";
            this.hitSoundString = "";
            this.muzzleString = ChooseAnimationString();
            this.swingEffectPrefab = Modules.Assets.shiggySwingEffect;
            this.hitEffectPrefab = Modules.Assets.shiggyHitImpactEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            base.OnEnter();

        }


        private string ChooseAnimationString()
        {
            string returnVal = "SwordSwing1";
            switch (this.swingIndex)
            {
                case 0:
                    returnVal = "SwordSwing1";
                    break;
                case 1:
                    returnVal = "SwordSwing2";
                    break;
                case 2:
                    returnVal = "SwordSwing3";
                    break;
            }

            return returnVal;
        }


        protected override void PlayAttackAnimation()
        {
            base.PlayAttackAnimation();
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

            if (base.isAuthority && base.IsKeyDownAuthority())
            {
                int index = this.swingIndex;
                index += 1;
                if (index > 2)
                {
                    index = 0;
                }
                this.outer.SetNextState(new SwordNeutral
                {
                    swingIndex = index
                });

            }

            return;

        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}



