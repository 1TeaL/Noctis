using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using System.Runtime.CompilerServices;
using NoctisMod.Modules;
using System.Reflection;

namespace NoctisMod.SkillStates
{
    public class GreatswordBackward2 : BaseMeleeAttack
    {
        public float damageMult;
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.greatswordSkillDef;
            this.hitboxName = "GreatswordHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = damageMult;
            this.procCoefficient = 1f;
            this.pushForce = 500f * damageMult;
            this.bonusForce = characterBody.characterDirection.forward * pushForce;
            this.baseDuration = 3.5f;
            this.attackStartTime = 0.15f;
            this.attackEndTime = 0.45f;
            this.baseEarlyExitTime = 0.45f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "SwordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingLeft";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            base.OnEnter();
            this.animator.SetBool("releaseChargeSlash", true);
            if (base.isAuthority)
            {
                if (Modules.Config.allowVoice.Value) { AkSoundEngine.PostEvent("NoctisVoice", base.gameObject); }
            }

            AkSoundEngine.PostEvent("GreatswordSwingSFX", base.gameObject);

        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "GSChargeSlash", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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

                GreatswordCombo GreatswordCombo = new GreatswordCombo();
                this.outer.SetNextState(GreatswordCombo);
                return;
            }

        }

        public override void OnExit()
        {
            base.OnExit();
            this.animator.SetBool("releaseChargeSlash", false);
            this.animator.SetBool("releaseChargeLeap", false);
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.damageMult);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.damageMult = reader.ReadInt64();
        }
    }

}




