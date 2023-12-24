using R2API;
using System;
using UnityEngine.Bindings;

namespace NoctisMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Noctis
            string prefix = NoctisPlugin.developerPrefix + "_NOCTIS_BODY_";

            string desc = $"Noctis is a technical melee focused survivor, switching between different weapons for different situations.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > Every weapon has neutral, forwards and backwards inputs." + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > Using a different weapon causes a unique swap attack to be used, for each input as well." + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > Utilise jumping, utility skill and attacks to cancel the end lag of attacks.." + Environment.NewLine + Environment.NewLine;
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
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Power of Kings");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"Noctis has a <style=cIsUtility>mana bar which regenerates over time and through each hit. He is able to jump cancel and dodge cancel his attacks, consuming {StaticValues.dodgeCost} mana. Cooldown reduction items reduce mana costs. Stock-based items and levels increase max mana</style>. Attacks can also cancel into attacks. " + Environment.NewLine +
                "<style=cIsUtility>He can sprint in any direction. He has a double jump.</style>");
            #endregion

            #region Base Skills
            LanguageAPI.Add(prefix + "SWORD_NAME", "Sword");
            LanguageAPI.Add(prefix + "SWORD_DESCRIPTION", $"Swing your sword. There are neutral, forward, backward and aerial attacks. Swapping into sword has different attacks and has invincibility. Most attacks will deal {StaticValues.swordDamage * 100f}% damage and proc of {StaticValues.swordProc}. ");

            LanguageAPI.Add(prefix + "GREATSWORD_NAME", "Greatsword");
            LanguageAPI.Add(prefix + "GREATSWORD_DESCRIPTION", $"<style=cIsDamage>Apply Vulnerability to enemies, making them take {StaticValues.GSVulnerabilityDebuff * 100f}% additional damage per stack, additively</style>. <style=cIsUtility> Gain {StaticValues.GSArmor} armor for all attacks </style>. Swing your greatsword. There are neutral, forward, backward and aerial attacks. Swapping into greatsword has different attacks which stun. Most attacks will deal {StaticValues.GSDamage * 100f}% damage and proc of {StaticValues.GSProc}. ");

            LanguageAPI.Add(prefix + "POLEARM_NAME", "Polearm");
            LanguageAPI.Add(prefix + "POLEARM_DESCRIPTION", $"Swing your polearm. There are neutral, forward, backward, aerial attacks AND hold jump + aerial attacks. Most attacks will deal 2x{StaticValues.polearmDamage * 100f}% damage and proc of {StaticValues.polearmProc}. Swapping into polearm attacks have 3x{StaticValues.polearmDamage * 100f}% damage. ");

            LanguageAPI.Add(prefix + "DODGE_NAME", "Dodge");
            LanguageAPI.Add(prefix + "DODGE_DESCRIPTION", $"<style=cIsUtility>Dash in the direction or airstep in the direction if in the air, granting {StaticValues.dodgeArmor} armor during the dodge</style>. Useable during weapon attacks to cancel the end lag. Costs {StaticValues.dodgeCost} mana, reducible by CDR items. ");

            LanguageAPI.Add(prefix + "WARPSTRIKE_NAME", "Warpstrike");
            LanguageAPI.Add(prefix + "WARPSTRIKE_DESCRIPTION", $"<style=cIsUtility>Warp to where to a Target or where you're aiming.</style> When arriving at an enemy, deal <style=cIsDamage>damage based on distance travelled</style>. Costs {StaticValues.warpstrikeCost} mana, reducible by CDR items. ");

            LanguageAPI.Add(prefix + "ARMIGER_NAME", "Armiger");
            LanguageAPI.Add(prefix + "ARMIGER_DESCRIPTION", $"<style=cIsUtility>Activate Armiger, destroying all nearby projectiles while the buff is active.</style> Costs all your mana, for each {StaticValues.armigerThreshold} mana spent, gain 1 second of the Armiger buff. Stackable. ");

            #endregion


            #region Achievements
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Noctis: Mastery");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESCRIPTION", "As Noctis, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Noctis: Mastery");
            #endregion


            #region Keywords
            LanguageAPI.Add(prefix + "KEYWORD_SWORD", $"Neutral: 3 Hit Combo dealing <style=cIsDamage>{StaticValues.swordNeutralDamage1 * 100f}%, {StaticValues.swordNeutralDamage2 * 100f}% and {StaticValues.swordNeutralDamage3 * 100f}% damage</style>. " + Environment.NewLine +
                $"Forward: Leap forward and slash, dealing <style=cIsDamage>{StaticValues.swordDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Backward: Backflip into the air, dealing <style=cIsDamage>{StaticValues.swordDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Aerial: Dash and slash towards a target, dealing <style=cIsDamage>{StaticValues.swordDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Neutral:<style=cIsUtility>Invincibility</style>. Slash up then down, dealing <style=cIsDamage>2x{StaticValues.swordDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Forward:<style=cIsUtility>Invincibility</style>. Dash forward very quickly, dealing <style=cIsDamage>2x{StaticValues.swordDamage * 100f}% damage to enemies behind you</style>. " + Environment.NewLine +
                $"Swap Backward:<style=cIsUtility>Invincibility</style>. Backflip into the air, dealing <style=cIsDamage>{StaticValues.swordDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Aerial:<style=cIsUtility>Invincibility</style>. Dash and slash in a direction. Backflip on contact, dealing <style=cIsDamage>{StaticValues.swordDamage * 100f}% damage</style>. ");

            LanguageAPI.Add(prefix + "KEYWORD_GREATSWORD", $"Neutral: Hold the button to enter a counter stance, gaining an <style=cIsUtility>additional {StaticValues.GSArmor} armor</style>. When hit, counter with a slam on the ground, dealing <style=cIsDamage>{StaticValues.GSCounterDamage * 100f}% damage</style>. Invincible during the slam. Release the button or press any other button to cancel it. " + Environment.NewLine +
                $"Forward: Leap and slam the ground, dealing <style=cIsDamage>{StaticValues.GSDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Backward: Charge your greatsword. On release, knock back enemies in front of you, dealing <style=cIsDamage>{StaticValues.GSDamage* 100f}% - {3*StaticValues.GSDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Aerial: Swing your greatsword while falling, dealing <style=cIsDamage>{StaticValues.GSDamage* 100f}% damage</style>. Deal damage based on how long you were falling when you hit the ground. " + Environment.NewLine +
                $"Swap Neutral:<style=cIsUtility>Stun</style>. Uppercut, launching the enemy up, dealing <style=cIsDamage>{StaticValues.GSDamage * 100f}% damage</style>. Continue to hold the input to warp to the target and slam the target down, dealing <style=cIsDamage>{StaticValues.GSDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Forward:<style=cIsUtility>Stun</style>. Do a fast overhead swing, dealing <style=cIsDamage>{StaticValues.GSSwapForwardDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Backward:<style=cIsUtility>Stun</style>. Charge your greatsword. On release, leap and slam the ground dealing <style=cIsDamage>{StaticValues.GSDamage * 100f}% - {3 * StaticValues.GSDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Aerial:<style=cIsUtility>Stun</style>. Warp to the Target and uppercut them, launching the enemy up, dealing <style=cIsDamage>{StaticValues.GSDamage * 100f}% damage</style>. Continue to hold the input to warp to the target and slam the target down, dealing <style=cIsDamage>{StaticValues.GSDamage * 100f}% damage</style>. ");

            LanguageAPI.Add(prefix + "KEYWORD_POLEARM", $"Neutral: Thrust forward dealing <style=cIsDamage>2x{StaticValues.polearmDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Forward: Charge, then dash and thrust forward dealing <style=cIsDamage>2x{StaticValues.polearmDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Backward: Throw your polearm, piercing and dealing <style=cIsDamage>2x{StaticValues.polearmDamage * 100f}% damage</style>.   " + Environment.NewLine +
                $"Aerial: Dash and thrust forward at your aim direction. Backhop on contact, dealing <style=cIsDamage>2x{StaticValues.polearmDamage * 100f}% damage</style>.  " + Environment.NewLine +
                $"Aerial + Jump: Dragoon thrust, descending down, dealing <style=cIsDamage>2x{StaticValues.polearmDamage * 100f}% damage</style>. Deal damage based on how long you were falling when you hit the ground. " + Environment.NewLine +
                $"Swap Neutral: Sweep your polearm from right to left, dealing <style=cIsDamage>3x{StaticValues.polearmDamage * 100f}% damage</style>.  " + Environment.NewLine +
                $"Swap Forward: Dash and thrust forward, dealing <style=cIsDamage>3x{StaticValues.polearmDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Backward: Backhop, dealing <style=cIsDamage>3x{StaticValues.polearmDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Aerial: Dash and thrust towards at your aim direction, dealing <style=cIsDamage>3x{StaticValues.polearmDamage * 100f}% damage</style>. " + Environment.NewLine +
                $"Swap Aerial + Jump: Double Dragoon thrust, descending down, dealing <style=cIsDamage>3x{StaticValues.polearmDamage * 100f}% damage</style>. Deal damage based on how long you were falling when you hit the ground." );
            #endregion
            #endregion


        }
    }
}
