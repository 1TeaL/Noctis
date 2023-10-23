using EntityStates;
using IL.RoR2.Skills;
using NoctisMod.Modules.Survivors;
using R2API;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace NoctisMod.SkillStates.BaseStates
{
    public class BaseMeleeAttack : BaseSkillState
    {
        public int swingIndex;
        public NoctisController noctisCon;

        protected string hitboxName = "Sword";

        protected DamageType damageType = DamageType.Generic;
        protected float damageCoefficient = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseDuration = 1f;
        protected float attackStartTime = 0.2f;
        protected float attackEndTime = 0.4f;
        protected float baseEarlyExitTime = 0.4f;
        protected float hitStopDuration = 0.012f;
        protected float attackRecoil = 0.75f;
        protected float hitHopVelocity = 4f;
        protected bool cancelled = false;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound;

        public bool hasFired;
        private float hitPauseTimer;
        public OverlapAttack attack;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;

        private int attackAmount;
        private float partialAttack;
        private HitBoxGroup hitBoxGroup;

        public RoR2.Skills.SkillDef weaponDef;
        public NoctisController.WeaponType weaponType = NoctisController.WeaponType.NONE;

        public override void OnEnter()
        {
            base.OnEnter();
            noctisCon = base.gameObject.GetComponent<NoctisController>();
            noctisCon.WeaponAppear(baseDuration + 0.5f, weaponType);
            this.hasFired = false;
            this.animator = base.GetModelAnimator();
            base.StartAimMode(this.baseDuration, false);
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);

            attackAmount = (int)this.attackSpeedStat;
            if (attackAmount < 1)
            {
                attackAmount = 1;
            }
            partialAttack = (float)(this.attackSpeedStat - (float)attackAmount);

            hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }

            this.PlayAttackAnimation();

            this.attack = new OverlapAttack();
            this.attack.damageType = this.damageType;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = this.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = this.procCoefficient;
            this.attack.hitEffectPrefab = this.hitEffectPrefab;
            this.attack.forceVector = this.bonusForce;
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;

            //DamageAPI.AddModdedDamageType(this.attack, Modules.Damage.shiggyDecay);

        }

        protected virtual void PlayAttackAnimation()
        {
            //update animations one day
            base.PlayCrossfade("FullBody, Override", "SwordSlashNeutral" + (1 + swingIndex), "Attack.playbackRate", baseDuration, 0.05f);
            //base.PlayCrossfade("FullBody, Override", "Slam" , "Slash.playbackRate", this.duration/2, 0.05f);
        }

        public override void OnExit()
        {
            if (!this.hasFired && !this.cancelled) this.FireAttack();
            
            base.OnExit();

            //this.animator.SetBool("attacking", false);
        }

        protected virtual void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.muzzleString, true);
        }

        protected virtual void OnHitEnemyAuthority()
        {
            Util.PlaySound(this.hitSoundString, base.gameObject);

            if (!this.hasHopped)
            {
                if (base.characterMotor && !base.characterMotor.isGrounded && this.hitHopVelocity > 0f)
                {
                    base.SmallHop(base.characterMotor, this.hitHopVelocity);
                }

                this.hasHopped = true;
            }

            if (!this.inHitPause && this.hitStopDuration > 0f)
            {
                this.storedVelocity = base.characterMotor.velocity;
                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
                this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
                this.inHitPause = true;
            }
        }

        public void FireAttack()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);

                if (base.isAuthority)
                {
                    this.PlaySwingEffect();
                    base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                }

                if (base.isAuthority)
                {
                    for (int i = 0; i < attackAmount; i++)
                    {
                        // Create Attack, fire it, do the on hit enemy authority.
                        this.attack = new OverlapAttack();
                        this.attack.damageType = this.damageType;
                        this.attack.attacker = base.gameObject;
                        this.attack.inflictor = base.gameObject;
                        this.attack.teamIndex = base.GetTeam();
                        this.attack.damage = this.damageCoefficient * this.damageStat;
                        this.attack.procCoefficient = this.procCoefficient;
                        this.attack.hitEffectPrefab = this.hitEffectPrefab;
                        this.attack.forceVector = this.bonusForce;
                        this.attack.pushAwayForce = this.pushForce;
                        this.attack.hitBoxGroup = hitBoxGroup;
                        this.attack.isCrit = base.RollCrit();
                        this.attack.impactSound = this.impactSound;
                        if (this.attack.Fire())
                        {
                            this.OnHitEnemyAuthority();
                        }
                    }
                    if (partialAttack > 0.0f)
                    {
                        // Create Attack, fire it, do the on hit enemy authority, partaial damage on final 
                        this.attack = new OverlapAttack();
                        this.attack.damageType = this.damageType;
                        this.attack.attacker = base.gameObject;
                        this.attack.inflictor = base.gameObject;
                        this.attack.teamIndex = base.GetTeam();
                        this.attack.damage = this.damageCoefficient * this.damageStat * partialAttack;
                        this.attack.procCoefficient = this.procCoefficient * partialAttack;
                        this.attack.hitEffectPrefab = this.hitEffectPrefab;
                        this.attack.forceVector = this.bonusForce;
                        this.attack.pushAwayForce = this.pushForce;
                        this.attack.hitBoxGroup = hitBoxGroup;
                        this.attack.isCrit = base.RollCrit();
                        this.attack.impactSound = this.impactSound;
                        if (this.attack.Fire())
                        {
                            this.OnHitEnemyAuthority();
                        }
                    }
                }
            }

        }

        protected virtual void SetNextState()
        {
            if(base.isAuthority)
            {
                if (!this.hasFired) this.FireAttack();
                int index = this.swingIndex;
                if (index == 0) index = 1;
                else index = 0;
                this.outer.SetNextState(new BaseMeleeAttack
                {
                    swingIndex = index
                });
                return;

            }


        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

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

            if (this.stopwatch >= (this.baseDuration * this.attackEndTime))
            {
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

            if (this.stopwatch >= (this.baseDuration - this.baseEarlyExitTime) && base.isAuthority)
            {
                if (inputBank.skill3.down)
                {
                    this.outer.SetNextState(new Dodge());
                    return;
                }
            }

            if (this.stopwatch >= this.baseDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.swingIndex = reader.ReadInt32();
        }
    }
}