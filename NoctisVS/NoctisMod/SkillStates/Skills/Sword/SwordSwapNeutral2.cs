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
using NoctisMod.Modules;

namespace NoctisMod.SkillStates
{
    public class SwordSwapNeutral2 : BaseMeleeAttack
    {
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);

            weaponDef = Noctis.swordSkillDef;
            this.hitboxName = "SwordHitbox";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = StaticValues.swordDamage;
            this.procCoefficient = StaticValues.swordProc;
            this.pushForce = 0f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.6f;
            this.baseEarlyExitTime = 0.6f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "SwordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = $"SwordSwingDown";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1);

            base.OnEnter();

        }



        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "SwordUpDownSlashPart2", "Attack.playbackRate", baseDuration - baseEarlyExitTime, 0.05f);
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
                characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0);
                this.outer.SetNextState(new SwordCombo());
                return;

            }


        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0);
        }

    }
}



