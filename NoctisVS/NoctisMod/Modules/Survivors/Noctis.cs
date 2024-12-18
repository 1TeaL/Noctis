﻿using BepInEx.Configuration;
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
        internal static SkillDef greatswordSkillDef;
        internal static SkillDef polearmSkillDef;

        //utility
        internal static SkillDef dodgeSkillDef;
        internal static SkillDef warpstrikeSkillDef;
        internal static SkillDef armigerSkillDef;



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
            characterPortrait = Modules.NoctisAssets.LoadCharacterIcon("Noctis"),
            crosshair = Modules.NoctisAssets.LoadCrosshair("Standard"),
            damage = 12f,
            healthGrowth = 15f,
            healthRegen = 1f,
            jumpCount = 2,
            maxHealth = 162f,
            moveSpeed = 7f,
            subtitleNameToken = NoctisPlugin.developerPrefix + "_NOCTIS_BODY_SUBTITLE",
            //podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
        };

        internal override int mainRendererIndex { get; set; } = 1;

        public static Material bodyShapeMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("bodyShapeMat");
        public static Material bootsShapeMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("bootsShapeMat");
        public static Material eyeMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("eyeMat");
        public static Material faceMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("faceMat");
        public static Material hairMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("hairMat");
        public static Material shirtShapeMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("shirtShapeMat");
        public static Material buttonsMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("buttonsMat");
        public static Material ringMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("ringMat");
        public static Material teethMat = NoctisAssets.mainAssetBundle.LoadAsset<Material>("teethMat");

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
                    ignoreOverlays = true,
                },
                new CustomRendererInfo
                {
                    childName = "eyeShape",
                    material = eyeMat,
                    ignoreOverlays = true,
                },
                new CustomRendererInfo
                {
                    childName = "faceShape",
                    material = faceMat,
                },
                new CustomRendererInfo
                {
                    childName = "hairAShape",
                    material = hairMat,
                },
                new CustomRendererInfo
                {
                    childName = "hairBShape",
                    material = hairMat,
                },
                new CustomRendererInfo
                {
                    childName = "teethShape",
                    material = teethMat,
                    ignoreOverlays = true,
                },
                new CustomRendererInfo
                {
                    childName = "buttonShape",
                    material = buttonsMat,
                    ignoreOverlays = true,
                },
                new CustomRendererInfo
                {
                    childName = "ringShape",
                    material = ringMat,
                    ignoreOverlays = true,
                },
                new CustomRendererInfo
                {
                    childName = "shirtShape",
                    material = shirtShapeMat,
                },
        };



        internal override Type characterMainState { get; set; } = typeof(NoctisCharacterMain);

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

            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "SwordHitbox");

            Transform hitboxTransform2 = childLocator.FindChild("GreatswordHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform2, "GreatswordHitbox");

            Transform hitboxTransform3 = childLocator.FindChild("PolearmThrustHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform3, "PolearmThrustHitbox");

            Transform hitboxTransform4 = childLocator.FindChild("AOEHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform4, "AOEHitbox");

            Transform hitboxTransform5 = childLocator.FindChild("GreatswordBigHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform5, "GreatswordBigHitbox");

            //Transform hitboxTransform5 = childLocator.FindChild("DecayHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform5, "DecayHitbox");
        }



        internal override void InitializeSkills()
        {
            Skills.CreateSkillFamilies(bodyPrefab);
            Modules.Skills.CreateFirstExtraSkillFamily(bodyPrefab);
            Modules.Skills.CreateSecondExtraSkillFamily(bodyPrefab);

            string prefix = NoctisPlugin.developerPrefix + "_NOCTIS_BODY_";

            #region Passive
            SkillLocator skillloc = bodyPrefab.GetComponent<SkillLocator>();
            skillloc.passiveSkill.enabled = true;
            skillloc.passiveSkill.skillNameToken = prefix + "PASSIVE_NAME";
            skillloc.passiveSkill.skillDescriptionToken = prefix + "PASSIVE_DESCRIPTION";
            skillloc.passiveSkill.icon = Modules.NoctisAssets.mainAssetBundle.LoadAsset<Sprite>("NoctisPassive");
            //skillloc.passiveSkill.keywordToken = prefix + "KEYWORD_PASSIVE";
            #endregion

            #region Weapons

            swordSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {

                skillName = prefix + "SWORD_NAME",
                skillNameToken = prefix + "SWORD_NAME",
                skillDescriptionToken = prefix + "SWORD_DESCRIPTION",
                skillIcon = Modules.NoctisAssets.mainAssetBundle.LoadAsset<Sprite>("NoctisSword"),
                activationState = new SerializableEntityStateType(typeof(SwordCombo)),
                activationStateMachineName = "Body",
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
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new[] {prefix + "KEYWORD_SWORD"}
            });

            greatswordSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {

                skillName = prefix + "GREATSWORD_NAME",
                skillNameToken = prefix + "GREATSWORD_NAME",
                skillDescriptionToken = prefix + "GREATSWORD_DESCRIPTION",
                skillIcon = Modules.NoctisAssets.mainAssetBundle.LoadAsset<Sprite>("NoctisGreatsword"),
                activationState = new SerializableEntityStateType(typeof(GreatswordCombo)),
                activationStateMachineName = "Body",
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
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new[] { prefix + "KEYWORD_GREATSWORD" }
            });

            polearmSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {

                skillName = prefix + "POLEARM_NAME",
                skillNameToken = prefix + "POLEARM_NAME",
                skillDescriptionToken = prefix + "POLEARM_DESCRIPTION",
                skillIcon = Modules.NoctisAssets.mainAssetBundle.LoadAsset<Sprite>("NoctisPolearm"),
                activationState = new SerializableEntityStateType(typeof(PolearmCombo)),
                activationStateMachineName = "Body",
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
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new[] { prefix + "KEYWORD_POLEARM" }
            });
            #endregion            


            #region Utility
            dodgeSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "DODGE_NAME",
                skillNameToken = prefix + "DODGE_NAME",
                skillDescriptionToken = prefix + "DODGE_DESCRIPTION",
                skillIcon = Modules.NoctisAssets.mainAssetBundle.LoadAsset<Sprite>("NoctisDodge"),
                activationState = new SerializableEntityStateType(typeof(SkillStates.Dodge)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
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

            warpstrikeSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "WARPSTRIKE_NAME",
                skillNameToken = prefix + "WARPSTRIKE_NAME",
                skillDescriptionToken = prefix + "WARPSTRIKE_DESCRIPTION",
                skillIcon = Modules.NoctisAssets.mainAssetBundle.LoadAsset<Sprite>("NoctisWarpstrike"),
                activationState = new SerializableEntityStateType(typeof(Warpstrike)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 2f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

            });

            armigerSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "ARMIGER_NAME",
                skillNameToken = prefix + "ARMIGER_NAME",
                skillDescriptionToken = prefix + "ARMIGER_DESCRIPTION",
                skillIcon = Modules.NoctisAssets.mainAssetBundle.LoadAsset<Sprite>("NoctisArmiger"),
                activationState = new SerializableEntityStateType(typeof(Armiger)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
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
                greatswordSkillDef,
                polearmSkillDef,
            });
            Skills.AddSecondarySkills(this.bodyPrefab, new SkillDef[]
            {
                greatswordSkillDef,
                swordSkillDef,
                polearmSkillDef,
            });
            Skills.AddUtilitySkills(this.bodyPrefab, new SkillDef[]
            {
                dodgeSkillDef,
            });
            Skills.AddSpecialSkills(this.bodyPrefab, new SkillDef[]
            {
                polearmSkillDef,
                swordSkillDef,
                greatswordSkillDef,
            });
            Modules.Skills.AddFirstExtraSkills(bodyPrefab, new SkillDef[]
            {
                warpstrikeSkillDef,
            });
            Modules.Skills.AddSecondExtraSkills(bodyPrefab, new SkillDef[]
            {
                armigerSkillDef,
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
                NoctisAssets.mainAssetBundle.LoadAsset<Sprite>("NoctisBaseSkin"),
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

