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
        public ParticleSystem SpinningWeaponAura;

        //Weapons
        public GameObject swordWeapon;
        public GameObject greatswordWeapon;
        public GameObject polearmWeapon;
        public GameObject polearmDownRightWeapon;
        public GameObject polearmDownLeftWeapon;

        public GameObject currentWeapon;
        public SkinnedMeshRenderer currentWeaponSkinMesh;

        public bool isTransitioning;
        public float transitionTimer;
        public float weaponTimer;
        public WeaponType weaponState;

        public enum WeaponType : ushort
        {
            NONE = 1,
            SWORD = 2,
            GREATSWORD = 3,
            POLEARM = 4,
            POLEARML = 5,
            POLEARMR = 6,
        }



        public void Awake()
        {

            child = GetComponentInChildren<ChildLocator>();
            anim = GetComponentInChildren<Animator>();


            if (child)
            {
                SpinningWeaponAura = child.FindChild("SpinningWeaponAura").GetComponent<ParticleSystem>();
                swordWeapon = child.FindChild("Sword").gameObject;
                greatswordWeapon = child.FindChild("Greatsword").gameObject;
                polearmWeapon = child.FindChild("Polearm").gameObject;
                polearmDownLeftWeapon = child.FindChild("PolearmDownLeft").gameObject;
                polearmDownRightWeapon = child.FindChild("PolearmDownRight").gameObject;
            }

            SpinningWeaponAura.Stop();


            activeindicator = new Indicator(gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
            //On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            inputBank = gameObject.GetComponent<InputBankTest>();


        }

        public void DisableAllWeapons()
        {
            swordWeapon.SetActive(false);
            greatswordWeapon.SetActive(false);
            polearmWeapon.SetActive(false);
            polearmDownLeftWeapon.SetActive(false);
            polearmDownRightWeapon.SetActive(false);
        }

        public void WeaponAppear(float timer, WeaponType weaponType)
        {
            if(weaponType == weaponState)
            {
                //refresh time
                weaponTimer = timer;
            }
            else
            {
                weaponState = weaponType;
                switch (weaponType)
                {
                    case WeaponType.NONE:
                        DisableAllWeapons();
                        currentWeapon = null;
                        currentWeaponSkinMesh = null;
                        break;
                    case WeaponType.SWORD:
                        currentWeapon = swordWeapon;
                        break;
                    case WeaponType.GREATSWORD:
                        currentWeapon = greatswordWeapon;
                        break;
                    case WeaponType.POLEARM:
                        currentWeapon = polearmWeapon;
                        break;
                    case WeaponType.POLEARML:
                        currentWeapon = polearmDownLeftWeapon;
                        break;
                    case WeaponType.POLEARMR:
                        currentWeapon = polearmDownRightWeapon;
                        break;
                }
                if (currentWeapon)
                {
                    currentWeapon.SetActive(true);
                    currentWeaponSkinMesh = currentWeapon.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
                    isTransitioning = true;
                    transitionTimer = 0f;
                } 

            }
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

            weaponState = WeaponType.NONE;


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
            if (weaponState != WeaponType.NONE)
            {
                if(isTransitioning)
                {
                    if(transitionTimer < StaticValues.weaponTransitionThreshold)
                    {
                        transitionTimer += Time.deltaTime;
                        if(currentWeapon && currentWeaponSkinMesh)
                        {
                            Material[] Array = currentWeaponSkinMesh.materials;
                            Array[0].SetFloat("_transitionLerp", Mathf.Lerp(0f, 1f, transitionTimer / StaticValues.weaponTransitionThreshold));
                            currentWeaponSkinMesh.materials = Array;
                        }
                    }
                    else
                    {
                        isTransitioning = false;
                        transitionTimer = 0f;
                    }
                }

                weaponTimer -= Time.deltaTime;
                if (weaponTimer < 0f)
                {
                    weaponTimer = 0f;
                    weaponState = WeaponType.NONE;
                    if (currentWeapon)
                    {
                        currentWeapon.SetActive(false);
                        currentWeaponSkinMesh = null;
                        currentWeapon = null;
                    }
                }

            }
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


