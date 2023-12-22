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
        public float maxTrackingDistance = StaticValues.maxTrackingDistance;
        public float maxTrackingAngle = 50f;
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
        public ParticleSystem DashParticle;

        //Weapons
        public GameObject swordWeapon;
        public GameObject greatswordWeapon;
        public GameObject polearmWeapon;
        public GameObject polearmDownRightWeapon;
        public GameObject polearmDownLeftWeapon;

        public GameObject currentWeaponR;
        public SkinnedMeshRenderer currentWeaponSkinMeshR;
        public GameObject currentWeaponL;
        public SkinnedMeshRenderer currentWeaponSkinMeshL;

        public bool isTransitioningL;
        public float transitionTimerL;
        public bool isTransitioningR;
        public float transitionTimerR;
        public bool fadingL;
        public bool fadingR;

        public float weaponTimerR;
        public float weaponTimerL;
        public WeaponTypeR weaponStateR;
        public WeaponTypeL weaponStateL;

        public bool isSwapped;
        public float swappedTimer;

        public enum WeaponTypeR : ushort
        {
            NONE = 1,
            SWORD = 2,
            GREATSWORD = 3,
            POLEARM = 4,
            POLEARMR = 5,
        }

        public enum WeaponTypeL : ushort
        {
            NONE = 1,
            POLEARML = 2,
        }


        public void Awake()
        {

            child = GetComponentInChildren<ChildLocator>();
            anim = GetComponentInChildren<Animator>();


            if (child)
            {
                SpinningWeaponAura = child.FindChild("SpinningWeaponAura").GetComponent<ParticleSystem>();
                DashParticle = child.FindChild("DashParticle").GetComponent<ParticleSystem>();
                swordWeapon = child.FindChild("Sword").gameObject;
                greatswordWeapon = child.FindChild("Greatsword").gameObject;
                polearmWeapon = child.FindChild("Polearm").gameObject;
                polearmDownLeftWeapon = child.FindChild("PolearmDownLeft").gameObject;
                polearmDownRightWeapon = child.FindChild("PolearmDownRight").gameObject;
            }

            SpinningWeaponAura.Stop();
            DashParticle.Stop();


            activeindicator = new Indicator(gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
            //On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            inputBank = gameObject.GetComponent<InputBankTest>();


        }

        public void DisableAllWeaponsL()
        {
            currentWeaponL = null;

            polearmDownLeftWeapon.SetActive(false);
        }
        public void DisableAllWeaponsR()
        {
            currentWeaponR = null;

            swordWeapon.SetActive(false);
            greatswordWeapon.SetActive(false);
            polearmWeapon.SetActive(false);
            polearmDownRightWeapon.SetActive(false);
        }

        public void WeaponAppearL(float timer, WeaponTypeL weapTypeL)
        {
            if (weapTypeL == weaponStateL)
            {
                //refresh time
                weaponTimerL = timer;
            }
            else
            {
                DisableAllWeaponsL();
                weaponTimerL = timer;
                weaponStateL = weapTypeL;
                switch (weapTypeL)
                {
                    case WeaponTypeL.NONE:
                        currentWeaponL = null;
                        currentWeaponSkinMeshL = null;
                        break;
                    case WeaponTypeL.POLEARML:
                        currentWeaponL = polearmDownLeftWeapon;
                        break;
                }
                if (currentWeaponR)
                {
                    currentWeaponL.SetActive(true);
                    currentWeaponSkinMeshL = currentWeaponL.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
                    isTransitioningL = true;
                    transitionTimerL = 0f;
                    fadingL = false;
                }

            }
        }
        public void WeaponAppearR(float timer, WeaponTypeR weapTypeR)
        {
            if(weapTypeR == weaponStateR)
            {
                //refresh time
                weaponTimerR = timer;
            }
            else
            {
                DisableAllWeaponsR();
                weaponTimerR = timer;
                weaponStateR = weapTypeR;
                switch (weapTypeR)
                {
                    case WeaponTypeR.NONE:
                        currentWeaponR = null;
                        currentWeaponSkinMeshR = null;
                        break;
                    case WeaponTypeR.SWORD:
                        currentWeaponR = swordWeapon;
                        break;
                    case WeaponTypeR.GREATSWORD:
                        currentWeaponR = greatswordWeapon;
                        break;
                    case WeaponTypeR.POLEARM:
                        currentWeaponR = polearmWeapon;
                        break;
                    case WeaponTypeR.POLEARMR:
                        currentWeaponR = polearmDownRightWeapon;
                        break;
                }
                if (currentWeaponR)
                {
                    currentWeaponR.SetActive(true);
                    currentWeaponSkinMeshR = currentWeaponR.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
                    isTransitioningR = true;
                    transitionTimerR = 0f;
                    fadingR = false;
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

            weaponStateR = WeaponTypeR.NONE;
            weaponStateL = WeaponTypeL.NONE;


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
                    if(swappedTimer > 0f)
                    {
                        swappedTimer -= Time.fixedDeltaTime;
                        isSwapped = true;
                    }
                    else if (swappedTimer <= 0f)
                    {
                        isSwapped = false;
                    }
                }
            }
        }    

        public void SetSwapTrue(float timer)
        {
            swappedTimer = timer;
            if(!isSwapped)
            {
                isSwapped = true;
            }
        }


        public void Update()
        {
            if (weaponStateR != WeaponTypeR.NONE)
            {
                if(isTransitioningR)
                {
                    if(transitionTimerR < StaticValues.weaponTransitionThreshold)
                    {
                        transitionTimerR += Time.deltaTime;
                        if(currentWeaponR && currentWeaponSkinMeshR)
                        {
                            Material[] Array = currentWeaponSkinMeshR.materials;
                            Array[1].SetFloat("_FrenelMultiplier", Mathf.Lerp(0f, 4f, transitionTimerR / StaticValues.weaponTransitionThreshold));
                            currentWeaponSkinMeshR.materials = Array;
                        }
                    }
                    else
                    {
                        isTransitioningR = false;
                        transitionTimerR = 0f;
                    }
                }

                weaponTimerR -= Time.deltaTime;
                if (weaponTimerR < 0f)
                {
                    weaponTimerR = 0f;
                    weaponStateR = WeaponTypeR.NONE;
                    if (currentWeaponR)
                    {
                        fadingR = true;
                        isTransitioningR = true;
                        transitionTimerR = 0f;
                    }
                }

            }
            if (weaponStateL != WeaponTypeL.NONE)
            {
                if (isTransitioningL)
                {
                    if (transitionTimerL < StaticValues.weaponTransitionThreshold)
                    {
                        transitionTimerL += Time.deltaTime;
                        if (currentWeaponL && currentWeaponSkinMeshL)
                        {
                            Material[] Array = currentWeaponSkinMeshL.materials;
                            Array[1].SetFloat("_FrenelMultiplier", Mathf.Lerp(0f, 4f, transitionTimerL / StaticValues.weaponTransitionThreshold));
                            currentWeaponSkinMeshL.materials = Array;
                        }
                    }
                    else
                    {
                        isTransitioningL = false;
                        transitionTimerL = 0f;
                    }
                }

                weaponTimerL -= Time.deltaTime;
                if (weaponTimerL < 0f)
                {
                    weaponTimerL = 0f;
                    weaponStateL = WeaponTypeL.NONE;
                    if (currentWeaponL)
                    {
                        fadingL = true;
                        isTransitioningL = true;
                        transitionTimerL = 0f;
                    }
                }

            }

            if (fadingL) 
            {
                if (transitionTimerL < StaticValues.weaponTransitionThreshold) 
                {
                    transitionTimerL += Time.deltaTime;
                    if (currentWeaponL && currentWeaponSkinMeshL) 
                    {
                        Material[] Array = currentWeaponSkinMeshL.materials;
                        Array[1].SetFloat("_FrenelMultiplier", Mathf.Lerp(4f, 0f, transitionTimerL / StaticValues.weaponTransitionThreshold));
                        currentWeaponSkinMeshL.materials = Array;
                    }
                }

                if (transitionTimerL > StaticValues.weaponTransitionThreshold)
                {
                    currentWeaponL.SetActive(false);
                    currentWeaponSkinMeshL = null;
                    currentWeaponL = null;
                    fadingL = false;
                }
            }

            if (fadingR) 
            {
                if (transitionTimerR < StaticValues.weaponTransitionThreshold)
                {
                    transitionTimerR += Time.deltaTime;
                    if (currentWeaponR && currentWeaponSkinMeshR)
                    {
                        Material[] Array = currentWeaponSkinMeshR.materials;
                        Array[1].SetFloat("_FrenelMultiplier", Mathf.Lerp(4f, 0f, transitionTimerR / StaticValues.weaponTransitionThreshold));
                        currentWeaponSkinMeshR.materials = Array;
                    }
                }

                if (transitionTimerR > StaticValues.weaponTransitionThreshold) 
                {
                    currentWeaponR.SetActive(false);
                    currentWeaponSkinMeshR = null;
                    currentWeaponR = null;
                    fadingR = false;
                }
            }
        }


        private void SearchForTarget(Ray aimRay)
		{
			this.search.teamMaskFilter = TeamMask.GetEnemyTeams(characterBody.teamComponent.teamIndex);
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


