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
        //weapon timer
        internal static float weaponTransitionThreshold = 0.4f;

        //Energy
        internal static float baseMana = 100f;
        internal static float levelMana = 10f;
        internal static float regenManaRate = 8f;
        internal static float minimumManaCost = 1f;
        internal static float costFlatManaSpend = 5f;
        internal static float regenManaFraction = 0.025f;
        internal static float backupGain = 10f;
        internal static float afterburnerGain = 30f;
        internal static float lysateGain = 15f;

        //Sword
        internal static float swordProc = 1f;
        internal static float swordNeutralDamage1 = 2f;
        internal static float swordNeutralDamage2 = 1f;
        internal static float swordNeutralDamage3 = 4f;
        internal static float swordDashDistance = 3f;
        internal static float swordDashSpeed = 6f;
        internal static float swordInstaDashSpeed = 20f;
        internal static float swordBackSpeed = 6f;
        internal static float swordBaseHit = 2f;
        internal static float swordSwapForwardDamage = 2f;

        //GS
        internal static float GSDamage = 4f;
        internal static float GSLeapSpeed = 4f;
        internal static float GSMaxCharge = 3f;
        internal static float GSChargeDamage = 6f;
        internal static float GSChargeMultiplier = 2f;
        internal static float GSSlamRadius = 6f;
        internal static float GSDropSpeed = 60f;

        //Polearm
        internal static float polearmProc = 0.3f;
        internal static float polearmDamage = 1f;
        internal static int polearmExtraHit = 1;
        internal static int polearmSwapExtraHit = 2;
        internal static float polearmDashSpeed = 10f;
        internal static float polearmDropSpeed = 40f;
        internal static float polearmSlamRadius = 8f;
        internal static float polearmAerialDamage = 2f;
        internal static float polearmSwapBackwardDamage = 2f;

        //Dodge
        internal static float dodgeSpeed = 3f;
        internal static float dodgeArmor = 300f;
        internal static float dodgeHop = 10f;
        internal static float dodgeCost = 10f;

        //Warpstrike
        internal static float warpstrikeSpeed = 100f;
        internal static float warpstrikeDamageScaling = 0.3f;
        internal static float warpstrikeCost = 30f;
    }
}
