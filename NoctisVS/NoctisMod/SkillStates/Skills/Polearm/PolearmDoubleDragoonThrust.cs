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
    public class PolearmDoubleDragoonThrust : BaseMeleeAttack
    {
        private float dropTimer;
        private GameObject slamIndicatorInstance;

        private bool hasDropped;
        private bool hasImpacted;
        public float dropForce = StaticValues.polearmDropSpeed;

        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.polearmSkillDef;
            this.hitboxName = "AOEHitbox";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = StaticValues.polearmSlamDamage;
            this.procCoefficient = StaticValues.polearmProc;
            this.pushForce = 300f;
            this.baseDuration = 2f;
            this.attackStartTime = 0.46f;
            this.attackEndTime = 0.95f;
            this.baseEarlyExitTime = 0.3f;
            this.hitStopDuration = 0f;
            this.attackRecoil = 0f;
            this.hitHopVelocity = 0f;

            this.swingSoundString = "PolearmSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingDown";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffect;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;



            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.Motor.RebuildCollidableLayers();

            characterBody.ApplyBuff(Modules.Buffs.GSarmorBuff.buffIndex, 1);

            base.OnEnter();
            attackAmount += StaticValues.polearmSwapExtraHit;
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 2f);

            
            noctisCon.WeaponAppearL(2f, WeaponTypeL.POLEARML);
            noctisCon.WeaponAppearR(2f, WeaponTypeR.POLEARMR);

            SmallHop(characterMotor, 40f);

            if (isSwapped)
            {
                this.baseDuration = 1.6f;
                this.attackStartTime = 0f;
                this.attackEndTime = 0.95f;
                this.baseEarlyExitTime = 0.3f;
            }
        }

        public override void FixedUpdate()
        {

            dropTimer += Time.fixedDeltaTime;

            if (!this.hasDropped)
            {
                base.characterMotor.velocity.y = 0f;
            }

            if (base.fixedAge >= (baseDuration * attackStartTime * 0.25f) && !this.slamIndicatorInstance)
            {
                this.CreateIndicator();
            }

            if (base.fixedAge >= baseDuration * attackStartTime && !this.hasDropped)
            {
                this.StartDrop();
            }

            if (this.hasDropped && base.isAuthority && !base.characterMotor.disableAirControlUntilCollision)
            {
                this.LandingImpact();
                this.outer.SetNextStateToMain();
            }

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
                base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                this.inHitPause = false;
                base.characterMotor.velocity = this.storedVelocity;
            }

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("Attack.playbackRate", 0f);
                this.animator.SetFloat("Slash.playbackRate", 0f);
                this.animator.SetFloat("Swing.playbackRate", 0f);
            }

            if (this.stopwatch >= (this.baseDuration * this.attackStartTime) && this.stopwatch <= (this.baseDuration * this.attackEndTime))
            {
                this.FireAttack();
            }


            //movement cancel
            if (base.fixedAge >= baseDuration * attackEndTime * 1.3f)
            {

                if (base.inputBank.moveVector != Vector3.zero)
                {
                    this.outer.SetNextStateToMain();
                }
            }
            if (this.stopwatch >= (this.baseDuration * this.baseEarlyExitTime) && base.isAuthority)
            {
                if (inputBank.skill3.down)
                {
                    this.outer.SetNextState(new Dodge());
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
                    if (skillLocator.primary.skillDef == weaponDef)
                    {
                        SetNextState();
                    }
                    else
                    {
                        if (!this.hasFired) this.FireAttack();
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                if (inputBank.skill2.down)
                {
                    if (skillLocator.secondary.skillDef == weaponDef)
                    {
                        SetNextState();
                    }
                    else
                    {
                        if (!this.hasFired) this.FireAttack();
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                if (inputBank.skill4.down)
                {
                    if (skillLocator.special.skillDef == weaponDef)
                    {
                        SetNextState();
                    }
                    else
                    {
                        if (!this.hasFired) this.FireAttack();
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }

            if (this.stopwatch >= this.baseDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
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
                this.slamIndicatorInstance.transform.localScale = Vector3.one * StaticValues.polearmSlamRadius * (1 + dropTimer / 2) * attackSpeedStat;
                this.slamIndicatorInstance.transform.localPosition = base.transform.position;
            }
        }
        private void LandingImpact()
        {
            if (!hasImpacted)
            {
                hasImpacted = true;
                AkSoundEngine.PostEvent("SlamSFX", base.gameObject);
                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();

                    base.characterMotor.velocity *= 0.1f;

                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.radius = StaticValues.polearmSlamRadius * (1 + dropTimer / 2) * attackSpeedStat;
                    blastAttack.procCoefficient = procCoefficient;
                    blastAttack.position = base.characterBody.footPosition;
                    blastAttack.attacker = base.gameObject;
                    blastAttack.crit = base.RollCrit();
                    blastAttack.baseDamage = base.characterBody.damage * damageCoefficient * (1 + dropTimer / 2);
                    blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                    blastAttack.baseForce = pushForce * (1 + dropTimer);
                    blastAttack.teamIndex = base.teamComponent.teamIndex;
                    blastAttack.damageType = damageType;
                    blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                    for (int i = 0; i < attackAmount; i++)
                    {
                        blastAttack.Fire();
                    }

                    if (partialAttack > 0f)
                    {
                        blastAttack.baseDamage = base.characterBody.damage * damageCoefficient * (1 + dropTimer / 2) * partialAttack;
                        blastAttack.procCoefficient = procCoefficient * partialAttack;
                        blastAttack.Fire();
                    }

                    for (int i = 0; i < 3; i += 1)
                    {
                        Vector3 effectPosition = base.characterBody.footPosition + (UnityEngine.Random.insideUnitSphere * (StaticValues.GSSlamRadius * (1 + dropTimer) * 0.5f));
                        effectPosition.y = base.characterBody.footPosition.y;
                        EffectManager.SpawnEffect(EntityStates.BeetleGuardMonster.GroundSlam.slamEffectPrefab, new EffectData
                        {
                            origin = effectPosition,
                            scale = StaticValues.polearmSlamRadius * (1 + dropTimer / 2) * attackSpeedStat,
                        }, true);
                    }


                }
            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "PolearmDoubleDescendingThrust", "Attack.playbackRate", baseDuration, 0.05f);
            animator.Play("FullBody, Override.PolearmDoubleDescendingThrust", -1, 0.25f);
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

                characterBody.ApplyBuff(Modules.Buffs.GSarmorBuff.buffIndex, 0);
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
            LandingImpact();

            characterBody.ApplyBuff(Modules.Buffs.GSarmorBuff.buffIndex, 0);
            if (this.slamIndicatorInstance)
                this.slamIndicatorInstance.SetActive(false);
            EntityState.Destroy(this.slamIndicatorInstance);

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

    }
}



