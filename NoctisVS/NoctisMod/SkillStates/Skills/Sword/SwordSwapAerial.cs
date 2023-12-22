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
using EntityStates.Treebot.Weapon;

namespace NoctisMod.SkillStates
{
    public class SwordSwapAerial : BaseSkillState

    {
        public NoctisController noctisCon;
        public float previousMass;
        private Ray aimRay;
        private Vector3 aimRayDir;
        private Transform modelTransform;
        private CharacterModel characterModel;

        public static float hitExtraDuration = 0.44f;
        public static float minExtraDuration = 0.2f;
        public static float initialSpeedCoefficient = StaticValues.swordDashSpeed*2f;
        public static float SpeedCoefficient;
        public static float finalSpeedCoefficient = 0f;
        public static float bounceForce = 2000f;
        private Vector3 bounceVector;
        private float stopwatch;
        private OverlapAttack detector;
        private OverlapAttack attack;
        protected string hitboxName2 = "SwordHitbox";
        protected string hitboxName = "PolearmThrustHitbox";
        protected float procCoefficient = StaticValues.swordProc;
        protected float pushForce = 500f;
        protected Vector3 bonusForce = new Vector3(10f, 400f, 0f);
        protected float baseDuration = 0.7f;
        protected float attackStartTime = 0.2f;
        protected float attackEndTime = 0.9f;
        protected float baseEarlyExitTime = 0.4f;
        protected float hitStopDuration = 0.15f;
        protected float attackRecoil = 0.75f;
        protected float hitHopVelocity = 250f;
        private float hitPauseTimer;
        protected bool inHitPause;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;
        //public static GameObject muzzleEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashMageLightningLarge");
        //public static GameObject muzzleEffectPrefab2 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/effects/BoostJumpEffect");
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        private float extraDuration;
        private float rollSpeed;
        private Vector3 forwardDirection;
        private Animator animator;
        private Vector3 direction;
        private bool hasHopped;
        public float radius = 15f;

        public float fajin;
        protected DamageType damageType = DamageType.Generic;
        private BlastAttack blastAttack;


        public override void OnEnter()
        {
            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            this.aimRayDir = aimRay.direction;

            SpeedCoefficient = initialSpeedCoefficient;
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            float num2 = (num / base.characterBody.baseMoveSpeed - 1f) * 0.67f;
            float num3 = num2 + 1f;
            this.extraDuration = Math.Max(hitExtraDuration / (num3), minExtraDuration);


            base.characterBody.AddTimedBuffAuthority(RoR2Content.Buffs.HiddenInvincibility.buffIndex, baseDuration / 2);
            this.animator = base.GetModelAnimator();
            this.animator.SetBool("attacking", true);
            base.characterBody.SetAimTimer(baseDuration);
            HitBoxGroup hitBoxGroup = null;
            HitBoxGroup hitBoxGroup2 = null;
            Transform modelTransform = base.GetModelTransform();
            bool flag = modelTransform.gameObject.GetComponent<AimAnimator>();
            if (flag)
            {
                modelTransform.gameObject.GetComponent<AimAnimator>().enabled = false;
            }
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


            bool flag2 = modelTransform;
            if (flag2)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxName);
                hitBoxGroup2 = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxName2);
            }


            ChargeSonicBoom chargeSonicBoom = new ChargeSonicBoom();
            Util.PlaySound(chargeSonicBoom.sound, base.gameObject);
            this.attack = new OverlapAttack();
            this.attack.damageType = this.damageType;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = base.characterBody.damage * Modules.StaticValues.swordDamage * attackSpeedStat * (num3);
            this.attack.procCoefficient = this.procCoefficient * attackSpeedStat * (num3);
            this.attack.hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;
            this.attack.forceVector = this.bonusForce;
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = Modules.Assets.hitSoundEffect.index;


            this.detector = new OverlapAttack();
            this.detector.damageType = this.damageType;
            this.detector.attacker = base.gameObject;
            this.detector.inflictor = base.gameObject;
            this.detector.teamIndex = base.GetTeam();
            this.detector.damage = 0f;
            this.detector.procCoefficient = 0f;
            this.detector.hitEffectPrefab = null;
            this.detector.forceVector = Vector3.zero;
            this.detector.pushAwayForce = 0f;
            this.detector.hitBoxGroup = hitBoxGroup2;
            this.detector.isCrit = false;
            this.direction = base.GetAimRay().direction.normalized;
            base.characterDirection.forward = base.characterMotor.velocity.normalized;

            base.GetModelAnimator().SetFloat("Attack.playbackRate", attackSpeedStat);
            base.PlayCrossfade("FullBody, Override", "AerialOneHandStab", "Attack.playbackRate", baseDuration, 0.05f);

            AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject); 
            if (base.isAuthority)
            {
                AkSoundEngine.PostEvent("NoctisVoice", this.gameObject);
            }

            noctisCon.SetSwapTrue(baseDuration);

        }

        private void RecalculateRollSpeed()
        {
            float num = this.moveSpeedStat;
            bool isSprinting = base.characterBody.isSprinting;
            if (isSprinting)
            {
                num /= base.characterBody.sprintingSpeedMultiplier;
            }
            float num2 = (num / base.characterBody.baseMoveSpeed - 1f) * 0.67f;
            float num3 = num2 + 1f;
            this.rollSpeed = num3 * Mathf.Lerp(SpeedCoefficient, finalSpeedCoefficient, base.fixedAge / baseDuration);
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.RecalculateRollSpeed();

            this.FireAttack();
            this.hitPauseTimer -= Time.fixedDeltaTime;
            bool flag = this.hitPauseTimer <= 0f && this.inHitPause;
            if (flag)
            {
                base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                this.inHitPause = false;
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.ApplyForce(this.bounceVector, true, false);
                FireSonicBoom fireSonicBoom = new FireSonicBoom();
                Util.PlaySound(fireSonicBoom.sound, base.gameObject);
                this.attack.Fire(null);
            }
            bool flag2 = !this.inHitPause;
            if (flag2)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                base.characterDirection.forward = this.bounceVector * -1f;
                bool flag3 = base.characterMotor;
                if (flag3)
                {
                    base.characterMotor.velocity = Vector3.zero;
                }
                bool flag4 = this.animator;
                if (flag4)
                {
                    this.animator.SetFloat("Attack.playbackRate", 0f);
                }
            }
            bool flag5 = !this.hasHopped;
            if (flag5)
            {
                base.characterDirection.forward = this.direction;
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion += this.direction * this.rollSpeed * Time.fixedDeltaTime;
            }
            if (stopwatch >= baseDuration * attackEndTime)
            {

                if (base.isAuthority)
                {
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
                    if (base.inputBank.jump.down)
                    {
                        Jump Jump = new Jump();
                        this.outer.SetNextState(Jump);
                        return;
                    }

                }
            }

            if (base.isAuthority && this.stopwatch >= baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }

        }

        public override void OnExit()
        {
            Transform modelTransform = base.GetModelTransform();
            bool flag = modelTransform.gameObject.GetComponent<AimAnimator>();
            if (flag)
            {
                modelTransform.gameObject.GetComponent<AimAnimator>().enabled = false;
            }
            base.characterMotor.velocity /= 1.75f;
            this.animator.SetBool("attacking", false);
            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.forwardDirection);
        }


        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.forwardDirection = reader.ReadVector3();
        }

        private void FireAttack()
        {
            bool isAuthority = base.isAuthority;
            if (isAuthority)
            {
                List<HurtBox> list = new List<HurtBox>();
                bool flag = this.detector.Fire(list);
                if (flag)
                {
                    foreach (HurtBox hurtBox in list)
                    {
                        bool flag2 = hurtBox.healthComponent && hurtBox.healthComponent.body;
                        if (flag2)
                        {
                            this.ForceFlinch(hurtBox.healthComponent.body);
                        }
                    }
                    this.OnHitEnemyAuthority();
                }
            }
        }

        protected virtual void OnHitEnemyAuthority()
        {

            base.characterBody.SetAimTimer(2f);

            bool flag = !this.hasHopped;
            if (flag)
            {

                base.PlayCrossfade("FullBody, Override", "AerialBackflip", "Attack.playbackRate", baseDuration, 0.01f);
                this.stopwatch = baseDuration - this.extraDuration;

                bool flag3 = base.characterMotor;
                if (flag3)
                {
                    base.characterMotor.velocity = Vector3.zero;
                    this.bounceVector = base.GetAimRay().direction * -1f;
                    this.bounceVector.y = 0.5f;
                    this.bounceVector *= bounceForce;
                }
                bool flag4 = !this.inHitPause && this.hitStopDuration > 0f;
                if (flag4)
                {
                    this.storedVelocity = base.characterMotor.velocity;
                    this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Attack.playbackRate");
                    float num = this.moveSpeedStat;
                    bool isSprinting = base.characterBody.isSprinting;
                    if (isSprinting)
                    {
                        num /= base.characterBody.sprintingSpeedMultiplier;
                    }
                    float num2 = 1f + (num / base.characterBody.baseMoveSpeed - 1f);
                    this.hitPauseTimer = this.hitStopDuration / num2;
                    this.inHitPause = true;
                }
                this.hasHopped = true;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
        protected virtual void ForceFlinch(CharacterBody body)
        {
            SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
            bool flag = component == null;
            if (!flag)
            {
                bool canBeHitStunned = component.canBeHitStunned;
                if (canBeHitStunned)
                {
                    component.SetPain();
                    bool flag2 = body.characterMotor;
                    if (flag2)
                    {
                        body.characterMotor.velocity = Vector3.zero;
                    }
                }
            }
        }


    }
}



