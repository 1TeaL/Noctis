using R2API;
using System;
using UnityEngine.Bindings;

namespace NoctisMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Shiggy
            string prefix = NoctisPlugin.developerPrefix + "_NOCTIS_BODY_";

            string desc = $"Noctis is a high mobility melee focused survivor, switching between different weapons for different situations.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > Every weapon has neutral, forwards and backwards inputs." + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > Using a different weapon causes a unique swap attack to be used, for each input as well." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > The Mana bar in the middle is used for jump cancels, dodging and warp strikes. It regenerates over time and per hit" + Environment.NewLine + Environment.NewLine;



            string outro = "..and so he left, becoming the true king.";
            string outroFailure = "It's..more than I can take.";

            LanguageAPI.Add(prefix + "NAME", "Noctis Lucis Caelum");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "King of Lucis");
            LanguageAPI.Add(prefix + "LORE", "I am Noctis, prince of Lucis and king of fishing!");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Shiny");
            LanguageAPI.Add(prefix + "HANDLESS_SKIN_NAME", "Handless");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Power of kings");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"< ! > Noctis has a mana bar which regenerates over time and through each hit. He is able to jump cancel and dodge cancel his attacks, consuming {StaticValues.dodgeCost} mana. Attacks can also cancel into attacks.. " + Environment.NewLine +
                "<style=cIsUtility>He has a double jump.</style>");
            #endregion

            #region Base Skills
            LanguageAPI.Add(prefix + "DECAY_NAME", "Decay");
            LanguageAPI.Add(prefix + "DECAY_DESCRIPTION", $"" +
                $"Slam and <style=cWorldEvent>[Decay]</style> the ground/air around you, dealing <style=cIsDamage>{100f}% damage</style>. " + Environment.NewLine + Environment.NewLine +
                $"<style=cSub>Pairs with [Rex- Seed Barrage] to create [Decay Plus Ultra]</style> <style=cWorldEvent>[Decay]</style>");

            #endregion


            #region Achievements
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Shiggy: Mastery");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESCRIPTION", "As Shiggy, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Shiggy: Mastery");
            #endregion


            #region Keywords
            //LanguageAPI.Add(prefix + "KEYWORD_DECAY", $"<style=cKeywordName>Decay</style>Deal the higher of <style=cIsDamage>{100f *StaticValues.decayDamageCoefficient}% damage or {StaticValues.decayDamagePercentage * 100f}% of the enemy's max HP </style>per second for {StaticValues.decayDamageTimer} seconds. This spreads to nearby targets every {StaticValues.decayadditionalTimer} seconds." +
            //    $"Each <style=cStack>stack reduces movespeed and attackspeed by 4%</style>. " +
            //    $"<style=cDeath>Instakills</style> at {StaticValues.decayInstaKillThreshold} stacks.");
            //LanguageAPI.Add(prefix + "KEYWORD_PASSIVE", $"<style=cKeywordName>Plus Chaos Meter</style>"
            //    + "Shigaraki has a" + Helpers.Passive(" meter that regenerates over time and through killing enemies. Stealing quirks, giving quirks, and specific skills cost plus chaos") + "."
            //    + Environment.NewLine
            //    + Environment.NewLine
            //+ $"<style=cKeywordName>Decay</style>"
            //    + $"Melee skills/Overlap attacks apply Decay. Decay deals <style=cIsDamage>{100f *StaticValues.decayDamageCoefficient} damage</style> per second for {StaticValues.decayDamageTimer} seconds. This spreads to nearby targets every {StaticValues.decayadditionalTimer} seconds."
            //    + Environment.NewLine
            //    + Environment.NewLine
            //+ $"<style=cKeywordName>Air Walk</style>"
            //    + "<style=cIsUtility>Holding jump in the air after 0.5 seconds let's him fly, ascending for up to 3 seconds or slowing his descent while using a skill. After 3 seconds he can still ascend but can't be moving in a direction as well</style>."
            //    + Helpers.Passive(" Drains plus ultra"));
            #endregion
            #endregion


        }
    }
}
