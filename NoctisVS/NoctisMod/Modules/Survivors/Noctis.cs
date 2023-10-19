using BepInEx.Configuration;
using EntityStates;
using EntityStates.Railgunner.Scope;
using RoR2;
using RoR2.Skills;
using NoctisMod.SkillStates;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoctisMod.Modules.Survivors
{


    internal class Noctis : SurvivorBase
    {
        internal override string bodyName { get; set; } = "Noctis";

        //weapon skill def
        internal static SkillDef swordSkillDef;

        //utility
        internal static SkillDef dodgeSkillDef;



        internal override GameObject bodyPrefab { get; set; }
        internal override GameObject displayPrefab { get; set; }

        internal override float sortPosition { get; set; } = 100f;

        internal override ConfigEntry<bool> characterEnabled { get; set; }

        internal override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            armor = 10f,
            armorGrowth = 0.5f,
            bodyName = "NoctisBody",
            bodyNameToken = NoctisPlugin.developerPrefix + "_NOCTIS_BODY_NAME",
            bodyColor = Color.magenta,
            characterPortrait = Modules.Assets.LoadCharacterIcon("Noctis"),
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            damage = 5f,
            healthGrowth = 41f,
            healthRegen = 1f,
            jumpCount = 2,
            maxHealth = 141f,
            moveSpeed = 7f,
            subtitleNameToken = NoctisPlugin.developerPrefix + "_NOCTIS_BODY_SUBTITLE",
            //podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
        };

        internal override int mainRendererIndex { get; set; } = 1;

        public static Material bodyShapeMat = Assets.mainAssetBundle.LoadAsset<Material>("bodyShapeMat");
        public static Material bootsShapeMat = Assets.mainAssetBundle.LoadAsset<Material>("bootsShapeMat");
        public static Material eyeMat = Assets.mainAssetBundle.LoadAsset<Material>("eyeMat");
        public static Material faceMat = Assets.mainAssetBundle.LoadAsset<Material>("faceMat");
        public static Material hairMat = Assets.mainAssetBundle.LoadAsset<Material>("hairMat");
        public static Material shirtShapeMat = Assets.mainAssetBundle.LoadAsset<Material>("shirtShapeMat");
        public static Material buttonsMat = Assets.mainAssetBundle.LoadAsset<Material>("buttonsMat");
        public static Material ringMat = Assets.mainAssetBundle.LoadAsset<Material>("ringMat");
        public static Material teethMat = Assets.mainAssetBundle.LoadAsset<Material>("teethMat");
        public static Material hopooHairMat = Materials.CreateHopooMaterial("hairMat");

        internal override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {

                new CustomRendererInfo
                {
                    childName = "bodyShape",
                    material = bodyShapeMat,
                },
                new CustomRendererInfo
                {
                    childName = "bootsShape",
                    material = bootsShapeMat,
                },
                new CustomRendererInfo
                {
                    childName = "eyelashShape",
                    material = faceMat,
                },
                new CustomRendererInfo
                {
                    childName = "eyeShape",
                    material = eyeMat,
                },
                new CustomRendererInfo
                {
                    childName = "faceShape",
                    material = faceMat,
                },
                new CustomRendererInfo
                {
                    childName = "hairAShape",
                    material = hopooHairMat,
                },
                new CustomRendererInfo
                {
                    childName = "hairBShape",
                    material = hopooHairMat,
                },
                new CustomRendererInfo
                {
                    childName = "teethShape",
                    material = teethMat,
                },
                new CustomRendererInfo
                {
                    childName = "buttonShape",
                    material = buttonsMat,
                },
                new CustomRendererInfo
                {
                    childName = "ringShape",
                    material = ringMat,
                },
                new CustomRendererInfo
                {
                    childName = "shirtShape",
                    material = shirtShapeMat,
                },
        };



        internal override Type characterMainState { get; set; } = typeof(EntityStates.GenericCharacterMain);

        //item display stuffs
        internal override ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal override List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal override UnlockableDef characterUnlockableDef { get; set; }
        private static UnlockableDef masterySkinUnlockableDef;

        internal override void InitializeCharacter()
        {
            base.InitializeCharacter();
            bodyPrefab.AddComponent<NoctisController>();
        }

        internal override void InitializeUnlockables()
        {
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Achievements.MasteryAchievement>(true);
        }

        internal override void InitializeDoppelganger()
        {
            base.InitializeDoppelganger();
        }



        internal override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            //Transform hitboxTransform = childLocator.FindChild("SmallHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform, "SmallHitbox");

            //Transform hitboxTransform2 = childLocator.FindChild("DetectSmallHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform2, "DetectSmallHitbox");

            //Transform hitboxTransform3 = childLocator.FindChild("FrontHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform3, "FrontHitbox");

            //Transform hitboxTransform4 = childLocator.FindChild("AroundHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform4, "AroundHitbox");

            //Transform hitboxTransform5 = childLocator.FindChild("DecayHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform5, "DecayHitbox");
        }



        internal override void InitializeSkills()
        {
            Skills.CreateSkillFamilies(bodyPrefab);
            Modules.Skills.CreateFirstExtraSkillFamily(bodyPrefab);

            string prefix = NoctisPlugin.developerPrefix + "_NOCTIS_BODY_";

            #region Passive
            SkillLocator skillloc = bodyPrefab.GetComponent<SkillLocator>();
            skillloc.passiveSkill.enabled = true;
            skillloc.passiveSkill.skillNameToken = prefix + "PASSIVE_NAME";
            skillloc.passiveSkill.skillDescriptionToken = prefix + "PASSIVE_DESCRIPTION";
            skillloc.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("allforone");
            skillloc.passiveSkill.keywordToken = prefix + "KEYWORD_PASSIVE";
            #endregion

            #region Weapons

            swordSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {

                skillName = prefix + "DECAY_NAME",
                skillNameToken = prefix + "DECAY_NAME",
                skillDescriptionToken = prefix + "DECAY_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("decay"),
                activationState = new SerializableEntityStateType(typeof(SkillStates.SwordCombo)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            #endregion            


            #region Utility
            dodgeSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "AIRCANNON_NAME",
                skillNameToken = prefix + "AIRCANNON_NAME",
                skillDescriptionToken = prefix + "AIRCANNON_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("aircannon"),
                activationState = new SerializableEntityStateType(typeof(SkillStates.SwordCombo)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

            });

            #endregion

            #region Chosen Skills
            Skills.AddPrimarySkills(bodyPrefab, new SkillDef[]
            {
                swordSkillDef,
            });
            Skills.AddSecondarySkills(this.bodyPrefab, new SkillDef[]
            {
                swordSkillDef,
            });
            Skills.AddUtilitySkills(this.bodyPrefab, new SkillDef[]
            {
                dodgeSkillDef,
            });
            Skills.AddSpecialSkills(this.bodyPrefab, new SkillDef[]
            {
                swordSkillDef,
            });
            Modules.Skills.AddFirstExtraSkills(bodyPrefab, new SkillDef[]
            {
                swordSkillDef,
            });
            #endregion

        }


        internal override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(NoctisPlugin.developerPrefix + "_NOCTIS_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("NoctisBaseSkin"),
                defaultRendererinfos,
                mainRenderer,
                model);


            skins.Add(defaultSkin);
            #endregion


            //#region masteryskin
            //Material masteryMat = Modules.Assets.CreateMaterial("ShinyNoctisMat", 0f, Color.white, 1.0f);
            //CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[] {
            //    masteryMat,
            //    masteryMat,
            //    masteryMat,
            //    masteryMat,
            //});
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(NoctisPlugin.developerPrefix + "_NOCTIS_BODY_MASTERY_SKIN_NAME",
            //    Assets.mainAssetBundle.LoadAsset<Sprite>("ShiggyShinySkin"),
            //    masteryRendererInfos,
            //    mainRenderer,
            //    model,
            //    masterySkinUnlockableDef);

            //masterySkin.meshReplacements = new SkinDef.MeshReplacement[] {
            //    new SkinDef.MeshReplacement
            //    {
            //        mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("MeshShiggy"),
            //        renderer = defaultRenderers[instance.mainRendererIndex].renderer
            //    },
            //};
            //skins.Add(masterySkin);
            //#endregion

            skinController.skins = skins.ToArray();
        }


        internal override void SetItemDisplays()
        {

        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];

            return newRendererInfos;
        }
    }
}

