using EntityStates;
using R2API;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EntityStates.LunarExploderMonster;
using RoR2.Projectile;
using EntityStates.MiniMushroom;
using UnityEngine.Networking;
using ExtraSkillSlots;
using R2API.Networking;
using NoctisMod.Modules.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.UIElements;
using NoctisMod.SkillStates;
using IL.RoR2.Achievements.Bandit2;
using RoR2.Items;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
using static UnityEngine.ParticleSystem.PlaybackState;
using EntityStates.VoidMegaCrab.BackWeapon;
using RiskOfOptions.Components.Panel;
using Unity.Baselib.LowLevel;
using RoR2.CharacterAI;

namespace NoctisMod.Modules.Survivors
{
    public class NoctisController : MonoBehaviour
    {
        string prefix = NoctisPlugin.developerPrefix + "_NOCTIS_BODY_";


        

        private Ray downRay;
        public float maxTrackingDistance = 70f;
        public float maxTrackingAngle = 20f;
        public float trackerUpdateFrequency = 10f;
        private Indicator activeindicator;
        public HurtBox trackingTarget;
        public HurtBox Target;

        private CharacterBody characterBody;
        private InputBankTest inputBank;
        private float trackerUpdateStopwatch;
        private ChildLocator child;
        private Animator anim;
        private readonly BullseyeSearch search = new BullseyeSearch();
        private CharacterMaster characterMaster;

        public NoctisController Noctiscon;
        public EnergySystem energySystem;

        
        private ExtraInputBankTest extrainputBankTest;
        private ExtraSkillLocator extraskillLocator;



        //Particles
        //public ParticleSystem RARM;
        //public ParticleSystem LARM;
        //public ParticleSystem OFA;
        //public ParticleSystem FINALRELEASEAURA;
        //public ParticleSystem SWORDAURAL;
        //public ParticleSystem SWORDAURAR;

        //particle bools

        public weaponType weaponState;
        public enum weaponType : ushort
        {
            NONE = 1,
            SWORD = 2,
            GREATSWORD = 3,
            POLEARM = 4,
        }



        public void Awake()
        {

            child = GetComponentInChildren<ChildLocator>();
            anim = GetComponentInChildren<Animator>();


            if (child)
            {
                //LARM = child.FindChild("lArmAura").GetComponent<ParticleSystem>();
                //RARM = child.FindChild("rArmAura").GetComponent<ParticleSystem>();
                //OFA = child.FindChild("OFAlightning").GetComponent<ParticleSystem>();
                //FINALRELEASEAURA = child.FindChild("finalReleaseAura").GetComponent<ParticleSystem>();
                //SWORDAURAL = child.FindChild("WindSwordL").GetComponent<ParticleSystem>();
                //SWORDAURAR = child.FindChild("WindSwordR").GetComponent<ParticleSystem>();
            }

            //LARM.Stop();
            //RARM.Stop();
            //OFA.Stop();
            //FINALRELEASEAURA.Stop();
            //SWORDAURAL.Stop();
            //SWORDAURAR.Stop();

            activeindicator = new Indicator(gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
            //On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            inputBank = gameObject.GetComponent<InputBankTest>();


        }


        public void Start()
        {
            characterBody = gameObject.GetComponent<CharacterBody>();
            characterMaster = characterBody.master;

            energySystem = gameObject.GetComponent<EnergySystem>();
            if (!energySystem)
            {
                energySystem = gameObject.AddComponent<EnergySystem>();
            }

           
            extraskillLocator = characterBody.gameObject.GetComponent<ExtraSkillLocator>();
            extrainputBankTest = characterBody.gameObject.GetComponent<ExtraInputBankTest>();

            weaponState = weaponType.NONE;


        }



        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        public void OnEnable()
        {
            this.activeindicator.active = true;
        }

        public void OnDisable()
        {
            this.activeindicator.active = false;
        }


        public void OnDestroy()
        {

        }


        public void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);
                HurtBox hurtBox = this.trackingTarget;
                if (hurtBox)
                {
                    this.activeindicator.active = true;
                    this.activeindicator.targetTransform = this.trackingTarget.transform;

                }
                else
                {
                    this.activeindicator.active = false;

                }

            }
            if (characterBody)
            {
                if (characterBody.hasEffectiveAuthority)
                {

                }
            }
        }    




        public void Update()
        {

        }


        private void SearchForTarget(Ray aimRay)
		{
			this.search.teamMaskFilter = TeamMask.all;
			this.search.filterByLoS = true;
			this.search.searchOrigin = aimRay.origin;
			this.search.searchDirection = aimRay.direction;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.maxTrackingDistance;
			this.search.maxAngleFilter = this.maxTrackingAngle;
			this.search.RefreshCandidates();
			this.search.FilterOutGameObject(base.gameObject);
			this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
		}


    }



}


