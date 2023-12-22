using EntityStates;
using ExtraSkillSlots;
using NoctisMod.Modules.Survivors;
using NoctisMod.Modules;
using R2API;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using R2API.Networking;

namespace NoctisMod.SkillStates
{
    internal class GreatswordCounterSlam : BaseSkillState
    {
        public ExtraInputBankTest extrainputBankTest;
        private ExtraSkillLocator extraskillLocator;
        public NoctisController noctisCon;
        private float baseDuration = 0.8f;
        internal float radius;
        internal Vector3 moveVec;
        private float baseForce = 600f;
        public float procCoefficient = StaticValues.GSProc;
        private Animator animator;
        private CharacterModel characterModel;
        private Transform modelTransform;

        public GameObject blastEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/effects/SonicBoomEffect");

        private float partialAttack;
        private float attackEndTime = 0.45f;
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

            this.animator.SetBool("releaseCounterSlam", true);
            base.PlayCrossfade("FullBody, Override", "GSDownSlam", "Attack.playbackRate", baseDuration, 0.05f);

            AkSoundEngine.PostEvent("NoctisHitSFX", base.gameObject);
            AkSoundEngine.PostEvent("SlamSFX", base.gameObject);

            {
                AkSoundEngine.PostEvent("NoctisVoice", this.gameObject);
            }

            noctisCon.SetSwapTrue(baseDuration);

            radius = StaticValues.GSSlamRadius * attackSpeedStat;


            attackAmount = (int)this.attackSpeedStat;
            if (attackAmount < 1)
            {
                attackAmount = 1;
            }
            partialAttack = (float)(this.attackSpeedStat - (float)attackAmount);

            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();

                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.3f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay2.duration = 0.3f;
                temporaryOverlay2.animateShaderAlpha = true;
                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay2.destroyComponentOnEnd = true;
                temporaryOverlay2.originalMaterial = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());

            }

            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1, 1);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

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
                blastAttack.baseDamage = this.damageStat * StaticValues.GSCounterDamage;
                blastAttack.baseForce = this.baseForce * StaticValues.GSCounterDamage;
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

            if (characterBody.HasBuff(Buffs.GSarmorBuff))
            {
                characterBody.ApplyBuff(Buffs.GSarmorBuff.buffIndex, 0);
            }
            if (characterBody.HasBuff(Buffs.armorBuff))
            {
                characterBody.ApplyBuff(Buffs.armorBuff.buffIndex, 0);
            }
            if (characterBody.HasBuff(Buffs.counterBuff))
            {
                characterBody.ApplyBuff(Buffs.counterBuff.buffIndex, 0);
            }
        }
    }
}