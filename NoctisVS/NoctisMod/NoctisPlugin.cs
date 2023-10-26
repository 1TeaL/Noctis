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
        public const string MODVERSION = "1.0.0";

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
            //On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
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

                //gain mana on hit
                if (attackerBody.baseNameToken == NoctisPlugin.developerPrefix + "_NOCTIS_BODY_NAME")
                {
                    EnergySystem energySys = attackerBody.GetComponent<EnergySystem>();

                    energySys.GainMana(energySys.maxMana * StaticValues.manaGainOnHit);

                }

                //vulnerability modded damage
                if (DamageAPI.HasModdedDamageType(damageInfo, Modules.Damage.noctisVulnerability))
                {
                    victimBody.ApplyBuff(Buffs.vulnerabilityDebuff.buffIndex, victimBody.GetBuffCount(Buffs.vulnerabilityDebuff) + 1);
                }

                if (victimBody.HasBuff(Buffs.vulnerabilityDebuff.buffIndex))
                {
                    damageInfo.damage += damageInfo.damage * victimBody.GetBuffCount(Buffs.vulnerabilityDebuff) * StaticValues.GSVulnerabilityDebuff;
                }

            }
            
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
                    AkSoundEngine.PostEvent("NoctisEntrance", self.gameObject);
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
