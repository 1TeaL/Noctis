using EntityStates;
using ExtraSkillSlots;
using NoctisMod.Modules;
using NoctisMod.Modules.Networking;
using NoctisMod.Modules.Survivors;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static NoctisMod.Modules.Survivors.NoctisController;
using Random = UnityEngine.Random;

namespace NoctisMod.SkillStates
{
    public class GreatswordNeutral : BaseSkillState
    {
        public NoctisController noctisCon;
        public ExtraInputBankTest extrainputBankTest;
        private ExtraSkillLocator extraskillLocator;
        public Animator animator;
        private CharacterModel characterModel;
        private Transform modelTransform;

        public enum DangerState {STARTBUFF, CHECKCOUNTER};
        public DangerState state;
        public DamageType damageType = DamageType.Freeze2s;

        public override void OnEnter()
        {
            base.OnEnter();
            noctisCon = GetComponent<NoctisController>();
            extraskillLocator = characterBody.gameObject.GetComponent<ExtraSkillLocator>();
            extrainputBankTest = characterBody.gameObject.GetComponent<ExtraInputBankTest>();

            this.animator = base.GetModelAnimator();
            this.animator.SetBool("releaseCounter", false);
            this.animator.SetBool("releaseCounterSlam", false);
            base.GetModelAnimator().SetFloat("Attack.playbackRate", 1f);

            base.PlayCrossfade("FullBody, Override", "GSCounterStance", "Attack.playbackRate", 1f, 0.05f);

            state = DangerState.STARTBUFF;

                                             
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            noctisCon.WeaponAppearR(1f, WeaponTypeR.GREATSWORD);


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

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {

            if (damageInfo != null && damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>())
            {
                bool flag = (damageInfo.damageType & DamageType.BypassArmor) > DamageType.Generic;
                if (!flag && damageInfo.damage > 0f)
                {
                    if (self.body.HasBuff(Modules.Buffs.counterBuff.buffIndex))
                    {
                        if (damageInfo.attacker != self)
                        {
                            self.TakeDamage(damageInfo);
                            self.body.ApplyBuff(Modules.Buffs.counterBuff.buffIndex, 0);
                            this.animator.SetBool("releaseCounterSlam", true);
                            new ForceCounterState(self.body.masterObjectId).Send(NetworkDestination.Clients);
                        }

                    }

                }
        

            }
            orig.Invoke(self, damageInfo);
        }


        public override void OnExit()
        {
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
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
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            switch (state)
            {
                case DangerState.STARTBUFF:
                    if (!base.characterBody.HasBuff(Modules.Buffs.counterBuff.buffIndex))
                    {
                        bool active = NetworkServer.active;
                        if (active)
                        {
                            base.characterBody.ApplyBuff(Modules.Buffs.counterBuff.buffIndex, 1);

                        }
                        state = DangerState.CHECKCOUNTER;
                    }
                    break;
                case DangerState.CHECKCOUNTER:
                    noctisCon.SetSwapTrue(1f);
                    noctisCon.WeaponAppearR(1f, WeaponTypeR.GREATSWORD);

                    base.GetModelAnimator().SetFloat("Attack.playbackRate", 0.01f);
                    if (inputBank.skill3.down)
                    {
                        this.animator.SetBool("releaseCounter", true);
                        this.outer.SetNextState(new Dodge());
                        return;
                    }
                    if (inputBank.jump.down)
                    {
                        this.animator.SetBool("releaseCounter", true);
                        this.outer.SetNextState(new Jump
                        {
                        });
                        return;
                    }
                    if (extrainputBankTest.extraSkill1.down)
                    {
                        Warpstrike warpstrike = new Warpstrike();
                        warpstrike.weaponSwap = true;
                        this.outer.SetNextState(warpstrike);
                        this.animator.SetBool("releaseCounter", true);
                        return;
                    }


                    if (inputBank.skill1.down)
                    {
                        if(skillLocator.primary.skillDef = Modules.Survivors.Noctis.greatswordSkillDef)
                        {
                            state = DangerState.CHECKCOUNTER;
                            this.animator.SetBool("releaseCounter", false);
                            this.animator.SetBool("releaseCounterSlam", false);
                        }
                        else
                        {
                            this.animator.SetBool("releaseCounter", true);
                            this.outer.SetNextStateToMain();
                            return;
                        }
                    }
                    else
                    if (inputBank.skill2.down)
                    {
                        if (skillLocator.secondary.skillDef = Modules.Survivors.Noctis.greatswordSkillDef)
                        {
                            state = DangerState.CHECKCOUNTER;
                            this.animator.SetBool("releaseCounter", false);
                            this.animator.SetBool("releaseCounterSlam", false);
                        }
                        else
                        {
                            this.animator.SetBool("releaseCounter", true);
                            this.outer.SetNextStateToMain();
                            return;
                        }
                    }
                    else
                    if (inputBank.skill4.down)
                    {
                        if (skillLocator.special.skillDef = Modules.Survivors.Noctis.greatswordSkillDef)
                        {
                            state = DangerState.CHECKCOUNTER;
                            this.animator.SetBool("releaseCounter", false);
                            this.animator.SetBool("releaseCounterSlam", false);
                        }
                        else
                        {
                            this.animator.SetBool("releaseCounter", true);
                            this.outer.SetNextStateToMain();
                            return;
                        }
                    }
                    else
                    {
                        this.animator.SetBool("releaseCounter", true);
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    break; 
            }

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}