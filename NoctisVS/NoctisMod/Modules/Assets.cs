using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using System.Collections.Generic;
using RoR2.UI;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
using System;

namespace NoctisMod.Modules
{
    internal static class Assets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        // particle effects
        internal static GameObject beam;
        internal static List<GameObject> networkObjDefs = new List<GameObject>();

        // networked hit sounds
        internal static NetworkSoundEventDef hitSoundEffect;

        // lists of assets to add to contentpack
        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();
        internal static List<EffectDef> effectDefs = new List<EffectDef>();

        // cache these and use to create our own materials
        internal static Shader hotpoo = RoR2.LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;
        private static string[] assetNames = new string[0];

        // CHANGE THIS
        private const string assetbundleName = "noctisassetbundle";


        //buffs
        //public static Sprite lunarRootIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/LunarSkillReplacements/bdLunarSecondaryRoot.asset").WaitForCompletion().iconSprite;
        //public static Sprite strongerBurnIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/StrengthenBurn/bdStrongerBurn.asset").WaitForCompletion().iconSprite;
        //public static Sprite mercExposeIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Merc/bdMercExpose.asset").WaitForCompletion().iconSprite;
        public static Sprite deathMarkDebuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/DeathMark/bdDeathMark.asset").WaitForCompletion().iconSprite;
        //public static Sprite singularityBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/ElementalRingVoid/bdElementalRingVoidReady.asset").WaitForCompletion().iconSprite;
        //public static Sprite cloakBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdCloak.asset").WaitForCompletion().iconSprite;
        //public static Sprite voidFogDebuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdVoidFogMild.asset").WaitForCompletion().iconSprite;
        //public static Sprite medkitBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Medkit/bdMedkitHeal.asset").WaitForCompletion().iconSprite;
        //public static Sprite spikyDebuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Treebot/bdEntangle.asset").WaitForCompletion().iconSprite;
        //public static Sprite ruinDebuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/LunarSkillReplacements/bdLunarDetonationCharge.asset").WaitForCompletion().iconSprite;
        //public static Sprite warcryBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/WarCryOnMultiKill/bdWarCryBuff.asset").WaitForCompletion().iconSprite;
        public static Sprite shieldBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdArmorBoost.asset").WaitForCompletion().iconSprite;
        //public static Sprite tarBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdClayGoo.asset").WaitForCompletion().iconSprite;
        //public static Sprite crippleBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdCripple.asset").WaitForCompletion().iconSprite;
        //public static Sprite speedBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Bandit2/bdCloakSpeed.asset").WaitForCompletion().iconSprite;
        //public static Sprite boostBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/RandomDamageZone/bdPowerBuff.asset").WaitForCompletion().iconSprite;
        //public static Sprite alphashieldonBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/BearVoid/bdBearVoidReady.asset").WaitForCompletion().iconSprite;
        //public static Sprite alphashieldoffBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/BearVoid/bdBearVoidCooldown.asset").WaitForCompletion().iconSprite;
        //public static Sprite decayBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdVoidFogStrong.asset").WaitForCompletion().iconSprite;
        //public static Sprite multiplierBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/PrimarySkillShuriken/bdPrimarySkillShurikenBuff.asset").WaitForCompletion().iconSprite;
        //public static Sprite jumpBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/MoveSpeedOnKill/bdKillMoveSpeed.asset").WaitForCompletion().iconSprite;
        //public static Sprite sprintBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/SprintOutOfCombat/bdWhipBoost.asset").WaitForCompletion().iconSprite;
        //public static Sprite spikeBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Grandparent/bdOverheat.asset").WaitForCompletion().iconSprite;
        //public static Sprite mortarBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/GainArmor/bdElephantArmorBoost.asset").WaitForCompletion().iconSprite;
        //public static Sprite healBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Croco/bdCrocoRegen.asset").WaitForCompletion().iconSprite;
        //public static Sprite attackspeedBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/EnergizedOnEquipmentUse/bdEnergized.asset").WaitForCompletion().iconSprite;
        //public static Sprite gravityBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/KillEliteFrenzy/bdNoCooldowns.asset").WaitForCompletion().iconSprite;
        //public static Sprite bleedBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdBleeding.asset").WaitForCompletion().iconSprite;
        //public static Sprite skinBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/OutOfCombatArmor/bdOutOfCombatArmorBuff.asset").WaitForCompletion().iconSprite;
        //public static Sprite orbreadyBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ElementalRings/bdElementalRingsReady.asset").WaitForCompletion().iconSprite;
        //public static Sprite orbdisableBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ElementalRings/bdElementalRingsCooldown.asset").WaitForCompletion().iconSprite;
        //public static Sprite blazingBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdOnFire.asset").WaitForCompletion().iconSprite;
        //public static Sprite lightningBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite;
        //public static Sprite resonanceBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/LaserTurbine/bdLaserTurbineKillCharge.asset").WaitForCompletion().iconSprite;
        //public static Sprite critBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite;
        //public static Sprite claygooBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdClayGoo.asset").WaitForCompletion().iconSprite;
        //public static Sprite predatorBuffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/AttackSpeedOnCrit/bdAttackSpeedOnCrit.asset").WaitForCompletion().iconSprite;

        //game material
        //public static Material alphaconstructShieldBuffMat = RoR2.LegacyResourcesAPI.Load<Material>("Materials/matEnergyShield");
        //public static Material alphaconstructShieldBuffMat;
        //public static Material blastingZoneBurnMat;



        //own effects
        //melee swing
        internal static GameObject noctisHitEffect;
        internal static GameObject noctisSwingEffect;

        //particles
        internal static GameObject noctisDashEffect;

        //fake projectiles
        internal static GameObject swordThrowParticle;
        internal static GameObject polearmThrowParticle;
        internal static GameObject polearmTracer;

        internal static void Initialize()
        {

            if (assetbundleName == "myassetbundle")
            {
                Debug.LogError("AssetBundle name hasn't been changed- not loading any assets to avoid conflicts");
                return;
            }

            LoadAssetBundle();
            LoadSoundbank();
            PopulateAssets();

        }

        internal static void LoadAssetBundle()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NoctisMod." + assetbundleName))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }
            Debug.Log(mainAssetBundle + "main asset bundle");
            assetNames = mainAssetBundle.GetAllAssetNames();
        }

        internal static void LoadSoundbank()
        {
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("NoctisMod.Noctis.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }

        internal static void PopulateAssets()
        {
            if (!mainAssetBundle)
            {
                Debug.LogError("There is no AssetBundle to load assets from.");
                return;
            }

            //sword swing
            noctisHitEffect = Assets.LoadEffect("hitEffect");
            noctisSwingEffect = Assets.LoadEffect("swingEffect", true);

            //particles
            noctisDashEffect = Assets.LoadEffect("DashParticle", true);

            //fake projectiles
            swordThrowParticle = Assets.LoadEffect("swordThrow", true);
            polearmThrowParticle = Assets.LoadEffect("polearmThrow", true);

            polearmTracer = LoadEffect("polearmParticle");

            if (!polearmTracer.GetComponent<EffectComponent>()) polearmTracer.AddComponent<EffectComponent>();
            if (!polearmTracer.GetComponent<VFXAttributes>()) polearmTracer.AddComponent<VFXAttributes>();
            if (!polearmTracer.GetComponent<NetworkIdentity>()) polearmTracer.AddComponent<NetworkIdentity>();

            Material bulletMat = null;

            foreach (LineRenderer i in polearmTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    bulletMat = UnityEngine.Object.Instantiate<Material>(i.material);
                    bulletMat.SetColor("_TintColor", new Color(0.68f, 0.58f, 0.05f));
                    i.material = bulletMat;
                    i.startColor = new Color(0.68f, 0.58f, 0.05f);
                    i.endColor = new Color(0.68f, 0.58f, 0.05f);

                }
            }
            Modules.Effects.AddEffect(polearmTracer);

            //sounds
            hitSoundEffect = CreateNetworkSoundEventDef("NoctisHitSFX");

        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (MeshRenderer i in objectToConvert.GetComponentsInChildren<MeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }

            foreach (SkinnedMeshRenderer i in objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        internal static Texture LoadCharacterIcon(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            bool assetExists = false;
            for (int i = 0; i < assetNames.Length; i++)
            {
                if (assetNames[i].Contains(resourceName.ToLower()))
                {
                    assetExists = true;
                    i = assetNames.Length;
                }
            }

            if (!assetExists)
            {
                Debug.LogError("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            effectDefs.Add(newEffectDef);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = Assets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat)
            {
                Debug.LogError("Failed to load material: " + materialName + " - Check to see that the name in your Unity project matches the one in this code");
                return commandoMat;
            }

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);
            
            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return Assets.CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return Assets.CreateMaterial(materialName, emission, Color.white);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}