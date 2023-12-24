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
using System.Linq;
using NoctisMod.Modules.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.PlayerLoop;

namespace NoctisMod.SkillStates
{
    public class GreatswordSwapNeutral : BaseMeleeAttack
    {
        private readonly BullseyeSearch search = new BullseyeSearch();
        public HurtBox target;
        private bool isTarget;
        public override void OnEnter()
        {

            //AkSoundEngine.PostEvent("SwordSwingSFX", base.gameObject);
            weaponDef = Noctis.greatswordSkillDef;
            this.hitboxName = "GreatswordHitbox";

            this.damageType = DamageType.Stun1s;

            this.damageCoefficient = 1f;
            this.procCoefficient = StaticValues.GSProc;
            this.pushForce = 1000f;
            this.bonusForce = Vector3.up;
            this.baseDuration = 2.7f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.45f;
            this.baseEarlyExitTime = 0.5f;
            this.hitStopDuration = 0.1f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 7f;

            this.swingSoundString = "GreatswordSwingSFX";
            this.hitSoundString = "";
            this.muzzleString = "SwordSwingUp";
            this.swingEffectPrefab = Modules.Assets.noctisSwingEffectMedium;
            this.hitEffectPrefab = Modules.Assets.noctisHitEffect;

            this.impactSound = Modules.Assets.hitSoundEffect.index;

            base.OnEnter();
            hasVulnerability = true;
            if (isSwapped)
            {
                this.baseDuration = 2f;
                this.attackStartTime = 0.05f;
                this.attackEndTime = 0.4f;
                this.baseEarlyExitTime = 0.45f;
            }

        }

        public void DealDamage()
        {

            search.teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam());
            search.filterByLoS = true;
            search.searchOrigin = characterBody.corePosition;
            search.searchDirection = base.GetAimRay().direction;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = 25f;
            search.maxAngleFilter = 360f;


            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);

            target = this.search.GetResults().FirstOrDefault<HurtBox>();

            if (target)
            {
                isTarget = true;
                new TakeDamageRequest(characterBody.masterObjectId, target.healthComponent.body.masterObjectId, damageStat * StaticValues.swordDamage, Vector3.up, true, true).Send(NetworkDestination.Clients);
            }


        }

        public override void FixedUpdate()
        {
            if (this.stopwatch >= (this.baseDuration * this.attackStartTime) && this.stopwatch <= (this.baseDuration * this.attackEndTime))
            {
                if (!isTarget)
                {
                    this.DealDamage();
                }
            }
            base.FixedUpdate();

        }

        protected override void PlayAttackAnimation()
        {
            if (isSwapped)
            {
                animator.Play("FullBody, Override.GSUpper", -1, 0.2f);
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "GSUpper", "Attack.playbackRate", this.baseDuration - this.baseEarlyExitTime, 0.05f);
            }
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

                if(isTarget && target.healthComponent.health > 1f)
                {
                    new ForceFollowUpState(characterBody.masterObjectId, target.healthComponent.body.masterObjectId).Send(NetworkDestination.Clients);
                    return;
                }
                else
                {
                    GreatswordCombo GreatswordCombo = new GreatswordCombo();
                    this.outer.SetNextState(GreatswordCombo);
                    return;

                }

            }

        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}



