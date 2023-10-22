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

            string desc = $"Shiggy is a multi-option survivor that can steal quirks from monster and base survivors to create his own playstyle.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > Steal quirk with {Config.AFOHotkey.Value}. Remove quirks with {Config.RemoveHotkey.Value}. Give quirks with {Config.AFOGiveHotkey.Value}. All rebindable in the configs." + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > There's also configs to enable ALL quirks selectable in the loadout if you'd like to choose them from the beginning." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Grabbing a quirk when owning a specific quirk already will create a combination, these combinations can further combine." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > The Plus Chaos Meter in the middle increases naturally and by killing enemies, it is used for All For One and certain skills." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Some quirks are passive buffs, while others are active skills." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Try out all the quirks and craft your ultimate build!" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Aim to get a mixture of base skills, synergy skills and ultimate skills as they aren't necessarily direct upgrades. For example, the Beetle Queen's Summon Ally quirk allows you to summon the base survivors, providing you more quirks." + Environment.NewLine + Environment.NewLine;



            string outro = "..and so he left, becoming the true king.";
            string outroFailure = "I even amaze myself sometimes...";

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
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "All For One");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"Steal quirks by looking at a target and pressing {Config.AFOHotkey.Value}. Remove them with {Config.RemoveHotkey.Value}. Give passive quirks to targets by pressing {Config.AFOGiveHotkey.Value}." + Environment.NewLine +
                Helpers.Passive("[Plus Chaos Meter] [Decay] [Air Walk]") + Environment.NewLine +
                "<style=cIsUtility>He has a double jump. He can sprint in any direction.</style>");
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
