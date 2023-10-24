using EntityStates;
using RoR2;
using UnityEngine;
using NoctisMod.Modules.Survivors;
using EntityStates.VoidSurvivor;
using NoctisMod.Modules;

namespace NoctisMod.SkillStates
{
    public class PolearmSwapBackward : BaseSkillState
    {
        private NoctisController noctisCon;
        private Animator animator;
        public float duration = 2f;
        private float fireTime;
        public bool hasFired;

        private float range = 100f;
        private float damageCoefficient = Modules.StaticValues.polearmSwapBackwardDamage;
        private float procCoefficient = Modules.StaticValues.polearmProc;
        private string muzzleString ="RHand";
        private int bulletcount;

        public override void OnEnter()
        {
            base.OnEnter();
            noctisCon = gameObject.GetComponent<NoctisController>();
            noctisCon.WeaponAppearR(3f, NoctisController.WeaponTypeR.POLEARMR);

            this.fireTime = 0.5f * this.duration;
            hasFired = false;
            Ray aimRay = base.GetAimRay();
            base.characterBody.SetAimTimer(this.duration);
            animator = base.GetModelAnimator();
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);
                       

            bulletcount = Mathf.RoundToInt(attackSpeedStat) + StaticValues.polearmSwapExtraHit;
            base.PlayCrossfade("FullBody, Override", "PolearmThrow", "Attack.playbackRate", duration, 0.05f);


        }

        public override void OnExit()
        {
            base.OnExit();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime && !hasFired)
            {
                noctisCon.currentWeaponR.SetActive(false);
                hasFired = true;
                Ray aimRay = base.GetAimRay();
                EffectManager.SpawnEffect(Assets.polearmThrowParticle, new EffectData
                {
                    origin = aimRay.origin,
                    rotation = Quaternion.LookRotation(new Vector3(aimRay.direction.x, aimRay.direction.y, aimRay.direction.z)),
                }, true);

                var bulletAttack = new BulletAttack
                {
                    bulletCount = (uint)bulletcount,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = this.damageStat * damageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = range,
                    force = 1000f,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 0f,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 1.5f,
                    sniper = true,
                    stopperMask = LayerIndex.noCollision.mask,
                    weapon = null,
                    //tracerEffectPrefab = Assets.polearmTracer,
                    spreadPitchScale = 0f,
                    spreadYawScale = 0f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,

                };
                bulletAttack.Fire();
            }


            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }




        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
