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
        //Energy
        internal static float basePlusChaos = 100f;
        internal static float levelPlusChaos = 10f;
        internal static float regenManaRate = 8f;
        internal static float basePlusChaosGain = 1f;
        internal static float killPlusChaosGain = 0.1f;
        internal static float minimumCostFlatPlusChaosSpend = 0.005f;
        internal static float costFlatPlusChaosSpend = 5f;
        internal static float costFlatContantlyDrainingCoefficient = 0.005f;
        internal static float regenManaFraction = 0.025f;
        internal static float backupGain = 10f;
        internal static float afterburnerGain = 30f;
        internal static float lysateGain = 15f;

        //Sword
        internal static float swordProc = 1f;
        internal static float swordNeutralDamage1 = 2f;
        internal static float swordNeutralDamage2 = 1f;
        internal static float swordNeutralDamage3 = 4f;
        internal static float swordDashDistance = 4f;
        internal static float swordDashSpeed = 6f;

    }
}
