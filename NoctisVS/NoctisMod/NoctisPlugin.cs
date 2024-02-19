using BepInEx;
using BepInEx.Bootstrap;
//using NoctisMod.Equipment;
//using NoctisMod.Items;
using NoctisMod.Modules;
//using NoctisMod.Modules.Networking;
using NoctisMod.Modules.Survivors;
using NoctisMod.SkillStates;
using EntityStates;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2.Orbs;
using EmotesAPI;
using EntityStates.JellyfishMonster;
using NoctisMod.Modules.Networking;
using System;
using RoR2.Items;
using R2API.Networking.Interfaces;
using EntityStates.VagrantMonster;
using R2API;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[module: UnverifiableCode]

namespace NoctisMod
{
    //neeed this!
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.sound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.networking", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.damagetype", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.dot", BepInDependency.DependencyFlags.HardDependency)]

    //don't need 
    //[BepInDependency("com.bepis.r2api.loadout", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.artifactcode", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.commandhelper", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.content_management", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.damagetype", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.deployable", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.difficulty", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.director", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.dot", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.elites", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.items", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.lobbyconfig", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.orb", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.lobbyconfig", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.sceneasset", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.tempvisualeffect", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.bepis.r2api.unlockable", BepInDependency.DependencyFlags.HardDependency)]

    [BepInDependency("com.KingEnderBrine.ExtraSkillSlots", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]

    
    public class NoctisPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said

        public static bool scepterInstalled = false;

        public NoctisController Noctiscon;

        public const string MODUID = "com.TeaL.NoctisMod";
        public const string MODNAME = "NoctisMod";
        public const string MODVERSION = "1.5.1";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string developerPrefix = "TEAL";

        internal List<SurvivorBase> Survivors = new List<SurvivorBase>();

        public static NoctisPlugin instance;
        public static CharacterBody NoctisCharacterBody;


        //public List<ItemBase> Items = new List<ItemBase>();
        //public List<EquipmentBase> Equipments = new List<EquipmentBase>();

        //public static Dictionary<ItemBase, bool> ItemStatusDictionary = new Dictionary<ItemBase, bool>();
        //public static Dictionary<EquipmentBase, bool> EquipmentStatusDictionary = new Dictionary<EquipmentBase, bool>();
        private BlastAttack blastAttack;
        private GameObject armigerEffectPrefab = Modules.Assets.armigerSwordParticle;

        private void Awake()
        {
            instance = this;
            NoctisCharacterBody = null;
            NoctisPlugin.instance = this;

            // load assets and read config
            Modules.Assets.Initialize();
            Modules.Config.ReadConfig();
            Modules.Damage.SetupModdedDamage(); //setup modded damage
            if (Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions")) //risk of options support
            {
                Modules.Config.SetupRiskOfOptions();
            }
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new Noctis().Initialize();

            //networking
            NetworkingAPI.RegisterMessageType<TakeDamageRequest>();
            NetworkingAPI.RegisterMessageType<SetFreezeOnBodyRequest>();
            NetworkingAPI.RegisterMessageType<ForceCounterState>();
            NetworkingAPI.RegisterMessageType<ForceFollowUpState>();
            NetworkingAPI.RegisterMessageType<ForceGSSwapAerial>();
            NetworkingAPI.RegisterMessageType<ExtractNetworkRequest>();


            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;

            Hook();

        }
        private void LateSetup(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            // have to set item displays later now because they require direct object references..
            Modules.Survivors.Noctis.instance.SetItemDisplays();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;
            //On.RoR2.CharacterBody.Start += CharacterBody_Start;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            //On.RoR2.TeleporterInteraction.FinishedState.OnEnter += TeleporterInteraction_FinishedState;
            //GlobalEventManager.onServerDamageDealt += GlobalEventManager_OnDamageDealt;
            //On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            //On.RoR2.CharacterBody.Update += CharacterBody_Update;
            //On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            //On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.CharacterModel.Awake += CharacterModel_Awake;
           

            if (Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI"))
            {
                On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
            }
        }



        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig.Invoke(self, damageInfo, victim);

            if (damageInfo.attacker)
            {

                var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                var victimBody = victim.GetComponent<CharacterBody>();

                //increased mana regen while in combat
                if (attackerBody.baseNameToken == NoctisPlugin.developerPrefix + "_NOCTIS_BODY_NAME")
                {
                    attackerBody.ApplyBuff(Buffs.manaBuff.buffIndex, 1, 2);
                }

                //vulnerability modded damage
                if (DamageAPI.HasModdedDamageType(damageInfo, Modules.Damage.noctisVulnerability))
                {
                    victimBody.ApplyBuff(Buffs.vulnerabilityDebuff.buffIndex, victimBody.GetBuffCount(Buffs.vulnerabilityDebuff) + 1);
                }

                //armiger extra damage buff
                if (attackerBody.HasBuff(Buffs.armigerBuff) && damageInfo.procCoefficient > 0f)
                {
                    Vector3 randRelPos = new Vector3((float)UnityEngine.Random.Range(5f, 6f), (float)UnityEngine.Random.Range(5f, 6f), (float)UnityEngine.Random.Range(5f, 6f));

                    EffectData effectData = new EffectData
                    {
                        scale = 1f,
                        origin = victimBody.corePosition + randRelPos,
                        rotation = Quaternion.LookRotation(victimBody.corePosition - (victimBody.corePosition+randRelPos))
                    };
                    if (Modules.Assets.armigerSwordParticle)
                    {
                        print("armiger effect spawn");
                        EffectManager.SpawnEffect(Modules.Assets.armigerSwordParticle, effectData, true);
                    }

                    var bulletAttack = new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = victimBody.corePosition - (victimBody.corePosition + randRelPos),
                        origin = victimBody.corePosition + randRelPos,
                        damage = damageInfo.damage * Modules.StaticValues.armigerDamageBonus,
                        damageColorIndex = DamageColorIndex.Fragile,
                        damageType = DamageType.Generic,
                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                        maxDistance = 10f,
                        force = 0f,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = attackerBody.RollCrit(),
                        owner = attackerBody.gameObject,
                        smartCollision = false,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = 0f,
                        radius = 1f,
                        sniper = false,
                        stopperMask = LayerIndex.noCollision.mask,
                        weapon = null,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Sniper.SniperWeapon.FireRifle.hitEffectPrefab,

                    };
                    bulletAttack.Fire();
                }

            }
            
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            //orig(self, damageInfo);


            if (self)
            {
                if (damageInfo.attacker)
                {
                    var victimBody = self.body;
                    var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody && victimBody)
                    {
                        if (victimBody.HasBuff(Buffs.vulnerabilityDebuff.buffIndex))
                        {
                            damageInfo.damage += damageInfo.damage * victimBody.GetBuffCount(Buffs.vulnerabilityDebuff) * StaticValues.GSVulnerabilityDebuff;
                        }

                    }

                }


            }

            orig(self, damageInfo);

        }

        private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self)
        {
            orig.Invoke(self);

            if (self)
            {
                if (self.baseNameToken == NoctisPlugin.developerPrefix + "_NOCTIS_BODY_NAME")
                {
                    NoctisController Noctiscon = self.gameObject.GetComponent<NoctisController>();

                    if (Modules.Config.allowVoice.Value)
                    {
                        AkSoundEngine.PostEvent("NoctisDeath", self.gameObject);
                    }
                    

                }
                

            }
        }

        private void CharacterModel_Awake(On.RoR2.CharacterModel.orig_Awake orig, CharacterModel self)
        {
            orig(self);
            if (self.gameObject.name.Contains("NoctisDisplay"))
            {
                if (Modules.Config.allowVoice.Value)
                {
                    AkSoundEngine.PostEvent("newNoctisEntrance", self.gameObject);
                }
                
            }

        }



        //EMOTES
        private void SurvivorCatalog_Init(On.RoR2.SurvivorCatalog.orig_Init orig)
        {
            orig();
            foreach (var item in SurvivorCatalog.allSurvivorDefs)
            {
                Debug.Log(item.bodyPrefab.name);
                if (item.bodyPrefab.name == "NoctisBody")
                {
                    CustomEmotesAPI.ImportArmature(item.bodyPrefab, Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("noctisHumanoid"));
                }
            }
        }

        private void GlobalEventManager_OnDamageDealt(DamageReport report)
        {

            bool flag = !report.attacker || !report.attackerBody;


        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            //buffs 
            if (self?.healthComponent)
            {
                orig.Invoke(self);
                if (self.HasBuff(Buffs.armorBuff))
                {
                    self.armor += StaticValues.dodgeArmor;
                }
                if (self.HasBuff(Buffs.counterBuff))
                {
                    self.armor += StaticValues.GSArmor;
                }
                if (self.HasBuff(Buffs.GSarmorBuff))
                {
                    self.armor += StaticValues.GSArmor;
                }

            }
            

        }


        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);

            if (self)
            {
                if (self.body)
                {
                    //this.OverlayFunction(Modules.Assets.alphaconstructShieldBuffMat, self.body.HasBuff(Modules.Buffs.alphashieldonBuff), self);
                }
            }
        }

        private void OverlayFunction(Material overlayMaterial, bool condition, CharacterModel model)
        {
            if (model.activeOverlayCount >= CharacterModel.maxOverlays)
            {
                return;
            }
            if (condition)
            {
                Material[] array = model.currentOverlays;
                int num = model.activeOverlayCount;
                model.activeOverlayCount = num + 1;
                array[num] = overlayMaterial;
            }
        }

    }
}
