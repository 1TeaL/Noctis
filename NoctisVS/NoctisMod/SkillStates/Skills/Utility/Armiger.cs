using NoctisMod.Modules.Survivors;
using EntityStates;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using NoctisMod.Modules;
using UnityEngine.Networking;
using RoR2.ExpansionManagement;
using ExtraSkillSlots;
using R2API.Networking;
using System;
using static UnityEngine.UI.Image;

namespace NoctisMod.SkillStates
{
    public class Armiger : BaseSkillState
    {
        private NoctisController noctisCon;
        private EnergySystem energySystem;
        public float baseDuration = 1f;
        private GameObject blastEffectPrefab = Assets.lightningNovaEffectPrefab;

        public override void OnEnter()
        {
            base.OnEnter();
            noctisCon = base.gameObject.GetComponent<NoctisController>();
            energySystem = base.gameObject.GetComponent<EnergySystem>();

            

            Ray aimRay = base.GetAimRay();
            EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
            {
                origin = base.characterBody.corePosition,
                scale = 1f,
                rotation = Quaternion.LookRotation(aimRay.direction),

            }, true);
            characterBody.ApplyBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 1, 1);

            AkSoundEngine.PostEvent("Armiger", base.gameObject);
            base.GetModelAnimator().SetFloat("Attack.playbackRate", attackSpeedStat);
            //base.PlayCrossfade("FullBody, Override", "FullBodyTheWorld", "Attack.playbackRate", duration, 0.05f);
            //base.PlayCrossfade("RightArm, Override", "R" + randomAnim, "Attack.playbackRate", duration, 0.05f);
            //AkSoundEngine.PostEvent("NoctisArmiger", base.gameObject);

            if(energySystem.currentMana >= energySystem.maxMana * 0.99f)
            {
                if(noctisCon.SpinningWeaponAura.isStopped)
                {
                    noctisCon.SpinningWeaponAura.Play();
                }

                int bufftoApply = Mathf.RoundToInt(energySystem.currentMana/StaticValues.armigerThreshold);
                int buffcount = characterBody.GetBuffCount(Buffs.armigerBuff);

                characterBody.ApplyBuff(Buffs.armigerBuff.buffIndex, buffcount + bufftoApply);

                energySystem.SpendMana(energySystem.currentMana);

            }
            else
            {
                this.outer.SetNextStateToMain();
                return;
            }

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(base.fixedAge > baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }

    }
}