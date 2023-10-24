using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using NoctisMod.SkillStates.BaseStates;
using R2API;
using R2API.Networking;
using EntityStates.Huntress;
using NoctisMod.Modules;
using static NoctisMod.Modules.Survivors.NoctisController;

namespace NoctisMod.SkillStates
{
    public class PolearmDragoonThrust : BaseMeleeAttack
    {
        private float dropTimer;
        private GameObject slamIndicatorInstance;

        private bool hasDropped;
        public float dropForce = StaticValues.polearmDropSpeed;

        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("ShiggyMelee", base.gameObject);
            weaponDef = Noctis.polearmSkillDef;
            this.hitboxName = "AOEHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = 2f;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.baseDuration = 2f;
            this.attackStartTime = 0.5f;
            this.attackEndTime = 0.9f;
            this.baseEarlyExitTime = 0.4f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "ShiggyMelee";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingDown";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;


            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.Motor.RebuildCollidableLayers();

            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 1);

            base.OnEnter();
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 2f);
            attackAmount += StaticValues.polearmExtraHit;
            noctisCon.WeaponAppearR(2f, WeaponTypeR.POLEARMR);

        }

        public override void FixedUpdate()
        {

            dropTimer += Time.fixedDeltaTime;

            if (!this.hasDropped)
            {
                base.characterMotor.rootMotion += Vector3.up* ((dropForce) * EntityStates.Mage.FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / baseDuration) * Time.fixedDeltaTime);
                base.characterMotor.velocity.y = 0f;
            }

            if (base.fixedAge >= (baseDuration * attackStartTime * 0.25f) && !this.slamIndicatorInstance)
            {
                this.CreateIndicator();
            }

            if (base.fixedAge >= baseDuration * attackStartTime * 0.5f && !this.hasDropped)
            {
                this.StartDrop();
            }

            if (this.hasDropped && base.isAuthority && !base.characterMotor.disableAirControlUntilCollision)
            {
                this.LandingImpact();
                this.outer.SetNextStateToMain();
            }

            base.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();

            if (this.slamIndicatorInstance) this.UpdateSlamIndicator();
        }

        private void StartDrop()
        {
            this.hasDropped = true;

            base.characterMotor.disableAirControlUntilCollision = true;
            base.characterMotor.velocity.y = -dropForce;
        }

        private void CreateIndicator()
        {
            if (EntityStates.Huntress.ArrowRain.areaIndicatorPrefab)
            {
                this.slamIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
                this.slamIndicatorInstance.SetActive(true);

            }
        }
        private void UpdateSlamIndicator()
        {
            if (this.slamIndicatorInstance)
            {
                this.slamIndicatorInstance.transform.localScale = Vector3.one * StaticValues.GSSlamRadius * (1 + dropTimer / 2) * attackSpeedStat;
                this.slamIndicatorInstance.transform.localPosition = base.transform.position;
            }
        }
        private void LandingImpact()
        {

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                base.characterMotor.velocity *= 0.1f;

                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = StaticValues.polearmSlamRadius * (1 + dropTimer / 2) * attackSpeedStat;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = base.characterBody.footPosition;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = base.RollCrit();
                blastAttack.baseDamage = base.characterBody.damage * damageCoefficient * StaticValues.polearmSlamRadius * (1 + dropTimer / 2) * attackSpeedStat;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = pushForce * (1 + dropTimer);
                blastAttack.teamIndex = base.teamComponent.teamIndex;
                blastAttack.damageType = damageType;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                for (int i =0; i <= attackAmount; i++)
                {
                    blastAttack.Fire();
                }

                for (int i = 0; i <= 4; i += 1)
                {
                    Vector3 effectPosition = base.characterBody.footPosition + (UnityEngine.Random.insideUnitSphere * (StaticValues.polearmSlamRadius * (1 + dropTimer) * 0.5f));
                    effectPosition.y = base.characterBody.footPosition.y;
                    EffectManager.SpawnEffect(EntityStates.BeetleGuardMonster.GroundSlam.slamEffectPrefab, new EffectData
                    {
                        origin = effectPosition,
                        scale = StaticValues.polearmSlamRadius * (1 + dropTimer / 2) * attackSpeedStat,
                    }, true);
                }


            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "PolearmJumpDescendingThrust", "Attack.playbackRate", baseDuration, 0.05f);
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

                characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 0);
                if (this.slamIndicatorInstance)
                    this.slamIndicatorInstance.SetActive(false);
                EntityState.Destroy(this.slamIndicatorInstance);

                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                base.gameObject.layer = LayerIndex.defaultLayer.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();

                PolearmCombo PolearmCombo = new PolearmCombo();
                this.outer.SetNextState(PolearmCombo);
                return;

            }


        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.ApplyBuff(Modules.Buffs.armorBuff.buffIndex, 0);
            if (this.slamIndicatorInstance)
                this.slamIndicatorInstance.SetActive(false);
            EntityState.Destroy(this.slamIndicatorInstance);

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

    }
}



