using IL.RoR2;
using IL.RoR2.Skills;
using On.RoR2.Skills;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoctisMod.Modules
{
    internal static class StaticValues
    {
        //indicator
        internal static float maxTrackingDistance = 80f;

        //weapon timer
        internal static float weaponTransitionThreshold = 0.4f;

        //Energy
        internal static float baseMana = 100f;
        internal static float levelMana = 5f;
        internal static float regenManaRate = 8f;
        internal static float minimumManaCost = 1f;
        internal static float costFlatManaSpend = 5f;
        //internal static float regenManaFraction = 0.0125f;
        internal static float regenManaFraction = 0.02f;
        internal static float backupGain = 5f;
        internal static float afterburnerGain = 15f;
        internal static float lysateGain = 10f;
        internal static float manaGainOnHit = 0.02f;
        internal static float manaRegenMultiplier = 2f;

        //Sword
        internal static float swordProc = 1f;
        internal static float swordDamage = 2f;
        internal static float swordNeutralDamage1 = 2f;
        internal static float swordNeutralDamage2 = 1f;
        internal static float swordNeutralDamage3 = 4f;
        internal static float swordDashDistance = 3f;
        internal static float swordDashSpeed = 42f;
        internal static float swordInstaDashSpeed = 40f;
        internal static float swordBackSpeed = 42f;
        internal static int swordBaseHit = 1;
        internal static float swordSwapForwardDamage = 2f;

        //GS
        internal static float GSArmor = 150f;
        internal static float GSProc = 2f;
        internal static float GSDamage = 4f;
        internal static float GSCounterDamage = 8f;
        internal static float GSSwapForwardDamage = 10f;
        internal static float GSLeapSpeed = 42f;
        internal static float GSMaxCharge = 3f;
        internal static float GSChargeRadius = 5f;
        internal static float GSChargeDamage = 4f;
        internal static float GSChargeMultiplier = 2f;
        internal static float GSSlamRadius = 8f;
        internal static float GSDropSpeed = 60f;
        internal static float GSVulnerabilityDebuff = 0.25f;

        //Polearm
        internal static float polearmProc = 0.3f;
        internal static float polearmDamage = 1.5f;
        internal static float polearmSlamDamage = 2.5f;
        internal static int polearmExtraHit = 1;
        internal static int polearmSwapExtraHit = 2;
        internal static float polearmDashSpeed = 40f;
        internal static float polearmDropSpeed = 40f;
        internal static float polearmSlamRadius = 8f;
        internal static float polearmAerialDamage = 2f;
        internal static float polearmSwapBackwardDamage = 2f;

        //Dodge
        internal static float dodgeSpeed = 6f;
        internal static float dodgeArmor = 300f;
        internal static float dodgeHop = 10f;
        internal static float dodgeCost = 10f;

        //Jump
        internal static float jumpSpeed = 1f;
        internal static float jumpHop = 20f;

        //Warpstrike
        internal static float warpstrikeSpeed = 120f;
        internal static float warpstrikeDamageScaling = 0.3f;
        internal static float warpstrikeCost = 30f;
        internal static float warpstrikeThreshold = 0.5f;
        internal static float warpstrikeFreezeRange = 30f;

        //Armiger
        internal static float armigerThreshold = 10f;
    }
}
