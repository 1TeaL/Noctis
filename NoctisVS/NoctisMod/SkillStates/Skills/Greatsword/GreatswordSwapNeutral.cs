using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using HG;
using NoctisMod.Modules;

namespace NoctisMod.SkillStates
{
    public class GreatswordSwapNeutral : BaseMeleeAttack
    {
        private bool hasSlammed;
        private float radius = StaticValues.GSSlamRadius;
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.greatswordSkillDef;
            this.hitboxName = "GreatswordHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = StaticValues.GSDamage;
            this.procCoefficient = StaticValues.GSProc;
            this.pushForce = 1000f;
            this.baseDuration = 4f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.6f;
            this.baseEarlyExitTime = 0.6f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "GreatswordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingUp";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;
            hasSlammed = false;
            base.OnEnter();
            if (base.isAuthority)
            {
                if (Modules.Config.allowVoice.Value) { AkSoundEngine.PostEvent("NoctisVoice", base.gameObject); }
            }

        }

        public override void FixedUpdate()
        {
            if(base.fixedAge > this.baseDuration * attackEndTime && !hasSlammed)
            {
                hasSlammed = true;

                EffectManager.SimpleMuzzleFlash(Assets.noctisSwingEffect, base.gameObject, "SwordSwingDown", true);

                for (int i = 0; i <= 4; i += 1)
                {
                    Vector3 effectPosition = base.characterBody.footPosition + (UnityEngine.Random.insideUnitSphere * radius / 2f);
                    effectPosition.y = base.characterBody.footPosition.y;
                    EffectManager.SpawnEffect(EntityStates.BeetleGuardMonster.GroundSlam.slamEffectPrefab, new EffectData
                    {
                        origin = effectPosition,
                        scale = radius / 2f,
                    }, true);
                }

                bool isAuthority = base.isAuthority;
                if (isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack();

                    blastAttack.position = base.characterBody.corePosition;
                    blastAttack.baseDamage = this.damageStat * damageCoefficient;
                    blastAttack.baseForce = this.pushForce;
                    blastAttack.radius = this.radius;
                    blastAttack.attacker = base.gameObject;
                    blastAttack.inflictor = base.gameObject;
                    blastAttack.teamIndex = base.teamComponent.teamIndex;
                    blastAttack.crit = base.RollCrit();
                    blastAttack.procChainMask = default(ProcChainMask);
                    blastAttack.procCoefficient = procCoefficient;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                    blastAttack.damageColorIndex = DamageColorIndex.Default;
                    blastAttack.damageType = DamageType.Stun1s;
                    blastAttack.attackerFiltering = AttackerFiltering.Default;
                    blastAttack.AddModdedDamageType(Modules.Damage.noctisVulnerability);

                    for (int i = 0; i <= attackAmount; i++)
                    {
                        blastAttack.Fire();
                    }
                    if (partialAttack > 0f)
                    {
                        blastAttack.baseDamage = this.damageStat * damageCoefficient * partialAttack;
                        blastAttack.procCoefficient = procCoefficient * partialAttack;
                        blastAttack.Fire();
                    }
                }
            }
            base.FixedUpdate();
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "GSUpDownSlam", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
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



