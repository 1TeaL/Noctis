using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using System.Collections.Generic;
using System.Linq;
using R2API.Networking;
using R2API.Networking.Interfaces;
using NoctisMod.Modules.Networking;
using NoctisMod.Modules;
using NoctisMod.Modules.Survivors;
using static UnityEngine.UI.Image;
using R2API;
using ExtraSkillSlots;

namespace NoctisMod.SkillStates
{
    public class GreatswordFollowUpSlam : BaseSkillState
    {
        public NoctisController noctisCon;
        public ExtraInputBankTest extrainputBankTest;
        private ExtraSkillLocator extraskillLocator;
        public bool hasTeleported;
        public bool hasFired;
        public float baseDuration = 1f;
        public float attackStartTime = 0.4f;
        public float earlyExitTime = 0.2f;


        private BlastAttack blastAttack;
        private Animator animator;
        private CharacterModel characterModel;
        private Transform modelTransform;
        private readonly BullseyeSearch search = new BullseyeSearch();

        public HurtBox Target;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            hasTeleported = false;


            noctisCon = gameObject.GetComponent<NoctisController>();
            extraskillLocator = characterBody.gameObject.GetComponent<ExtraSkillLocator>();
            extrainputBankTest = characterBody.gameObject.GetComponent<ExtraInputBankTest>();
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);
            Util.PlayAttackSpeedSound("GreatswordSwingSFX", base.gameObject, 1f);

            base.PlayCrossfade("FullBody, Override", "GSAerialSlash", "Attack.playbackRate", this.baseDuration, 0.05f);

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
        }


        public override void OnExit()
        {
            base.OnExit();
            this.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Target)
            {
                if (!hasTeleported)
                {
                    hasTeleported = true;
                    base.characterMotor.velocity = Vector3.zero;
                    base.characterMotor.Motor.SetPositionAndRotation(Target.healthComponent.body.transform.position + Vector3.up, Quaternion.LookRotation(base.GetAimRay().direction), true);
                    //new PerformDetroitTeleportNetworkRequest(base.characterBody.masterObjectId, Target.gameObject).Send(NetworkDestination.Clients);

                }

                //stop movement until hit
                if(base.fixedAge < baseDuration * attackStartTime)
                {
                    Target.healthComponent.body.characterMotor.velocity = Vector3.zero;
                    characterMotor.velocity = Vector3.zero;
                }
                
                if (base.fixedAge >= baseDuration * attackStartTime && !hasFired && base.isAuthority)
                {
                    hasFired = true;
                    EffectManager.SimpleMuzzleFlash(Modules.Assets.noctisSwingEffectMedium, base.gameObject, "SwordSwingDown", true);
                    AkSoundEngine.PostEvent("NoctisHitSFX", Target.gameObject);
                    new TakeDamageRequest(characterBody.masterObjectId, Target.healthComponent.body.masterObjectId, damageStat * StaticValues.GSDamage, Vector3.down, true, true).Send(NetworkDestination.Clients);

                    blastAttack = new BlastAttack();
                    blastAttack.radius = 10f;
                    blastAttack.procCoefficient = 1f;
                    blastAttack.position = base.transform.position;
                    blastAttack.damageType = DamageType.Stun1s;
                    blastAttack.attacker = base.gameObject;
                    blastAttack.crit = base.RollCrit();
                    blastAttack.baseDamage = base.damageStat;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                    blastAttack.baseForce = 1000f;
                    blastAttack.bonusForce = Vector3.down * 3000f;
                    blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                    blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                    DamageAPI.AddModdedDamageType(blastAttack, Damage.noctisVulnerability);

                    EffectManager.SpawnEffect(Modules.Assets.sonicboomEffectPrefab, new EffectData
                    {
                        origin = base.transform.position,
                        scale = 2f,
                        rotation = Quaternion.LookRotation(Vector3.down),

                    }, true);

                    blastAttack.Fire();
                }

                if(base.fixedAge >= baseDuration * earlyExitTime)
                {

                    if (extrainputBankTest.extraSkill1.down)
                    {
                        Warpstrike warpstrike = new Warpstrike();
                        warpstrike.weaponSwap = true;
                        this.outer.SetNextState(warpstrike);
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
                    if (inputBank.jump.down)
                    {
                        this.outer.SetNextState(new Jump
                        {
                        });
                        return;
                    }
                }

                if ((base.fixedAge >= this.baseDuration && base.isAuthority))
                {
                    this.outer.SetNextStateToMain();
                    return;
                }

            }
            else
            {
                base.skillLocator.utility.AddOneStock();
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
