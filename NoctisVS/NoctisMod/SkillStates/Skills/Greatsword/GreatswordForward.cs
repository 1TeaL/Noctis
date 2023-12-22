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
using ExtraSkillSlots;
using R2API.Networking;

namespace NoctisMod.SkillStates
{
    internal class GreatswordForward: BaseSkillState
    {
        public ExtraInputBankTest extrainputBankTest;
        private ExtraSkillLocator extraskillLocator;
        public NoctisController noctisCon;
        private float baseDuration = 2f;
        internal float radius;
        internal Vector3 moveVec;
        private float baseForce = 600f;
        public float procCoefficient = StaticValues.GSProc;
        private Animator animator;

        public GameObject blastEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/effects/SonicBoomEffect");

        private float rollSpeed;
        private float partialAttack;
        private float SpeedCoefficient;
        public static float initialSpeedCoefficient = Modules.StaticValues.GSLeapSpeed;
        private float finalSpeedCoefficient = 0f;
        private float attackEndTime = 0.36f;
        private bool hasFired;
        private Vector3 direction;
        private int attackAmount;

        public override void OnEnter()
        {

            base.OnEnter();

            noctisCon = gameObject.GetComponent<NoctisController>();
            extraskillLocator = characterBody.gameObject.GetComponent<ExtraSkillLocator>();
            extrainputBankTest = characterBody.gameObject.GetComponent<ExtraInputBankTest>();
            hasFired = false;
            this.animator = base.GetModelAnimator();
            this.animator.SetFloat("Attack.playbackRate", 1f);
            base.PlayCrossfade("FullBody, Override", "GSLeapSlash", "Attack.playbackRate", baseDuration, 0.05f);
            
            radius = StaticValues.GSSlamRadius * attackSpeedStat;
            attackAmount = (int)this.attackSpeedStat;
            if (attackAmount < 1)
            {
                attackAmount = 1;
            }
            partialAttack = (float)(this.attackSpeedStat - (float)attackAmount);
            SpeedCoefficient = initialSpeedCoefficient;
            this.direction = base.GetAimRay().direction.normalized;
            this.direction.y = 0f;
            AkSoundEngine.PostEvent("GreatswordSwingSFX", base.gameObject); if (base.isAuthority)
            {
                AkSoundEngine.PostEvent("NoctisVoice", this.gameObject);
            }

            noctisCon.SetSwapTrue(baseDuration);

        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            float num2 = (num / base.characterBody.baseMoveSpeed) * 0.67f;
            float num3 = num2 + 1f;
            this.rollSpeed = num3 * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / (this.baseDuration * this.attackEndTime));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            RecalculateRollSpeed();

            if (base.fixedAge <= (this.baseDuration * this.attackEndTime))
            {
                Vector3 velocity = this.direction * rollSpeed;
                velocity.y = base.characterMotor.velocity.y;
                base.characterMotor.velocity = velocity;
                base.characterDirection.forward = base.characterMotor.velocity.normalized;


            }
            if (base.fixedAge > this.baseDuration * this.attackEndTime)
            {
                if (!hasFired)
                {
                    FireAttack();
                    hasFired = true;
                }
                if (base.isAuthority)
                {
                    if (extrainputBankTest.extraSkill1.down)
                    {
                        Warpstrike warpstrike = new Warpstrike();
                        warpstrike.weaponSwap = true;
                        this.outer.SetNextState(warpstrike);
                        return;
                    }
                    if (inputBank.jump.down)
                    {
                        this.outer.SetNextState(new Jump
                        {
                        });
                        return;
                    }
                    if (inputBank.skill1.down)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    if (inputBank.skill2.down)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    if (inputBank.skill3.down)
                    {
                        this.outer.SetNextState(new Dodge());
                        return;
                    }
                    if (inputBank.skill4.down)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }

            if (base.fixedAge > this.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void FireAttack()
        {

            for (int i = 0; i < 3; i += 1)
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
                blastAttack.baseDamage = this.damageStat * StaticValues.GSDamage;
                blastAttack.baseForce = this.baseForce * StaticValues.GSDamage;
                blastAttack.radius = this.radius;
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = base.teamComponent.teamIndex;
                blastAttack.crit = base.RollCrit();
                blastAttack.procChainMask = default(ProcChainMask);
                blastAttack.procCoefficient = procCoefficient;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.damageColorIndex = DamageColorIndex.Default;
                blastAttack.damageType = DamageType.Generic;
                blastAttack.attackerFiltering = AttackerFiltering.Default;
                blastAttack.AddModdedDamageType(Modules.Damage.noctisVulnerability);

                for (int i = 0; i < attackAmount; i++)
                {
                    blastAttack.Fire();
                }
                if (partialAttack > 0f)
                {
                    blastAttack.baseDamage = this.damageStat * partialAttack;
                    blastAttack.procCoefficient = procCoefficient * partialAttack;
                    blastAttack.Fire();
                }
            }

        }

        public override void OnExit()
        {
            base.OnExit();
            this.animator.SetBool("releaseChargeSlash", false);
            this.animator.SetBool("releaseChargeLeap", false);

            if (characterBody.HasBuff(Buffs.GSarmorBuff))
            {
                characterBody.ApplyBuff(Buffs.GSarmorBuff.buffIndex, 0);
            }
            if (characterBody.HasBuff(Buffs.armorBuff))
            {
                characterBody.ApplyBuff(Buffs.armorBuff.buffIndex, 0);
            }
        }
    }
}



