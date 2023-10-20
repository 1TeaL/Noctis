using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoctisMod.Modules
{
    public static class Damage
    {
        internal static DamageAPI.ModdedDamageType noctisVulnerability;

        internal static void SetupModdedDamage()
        {
            noctisVulnerability = DamageAPI.ReserveDamageType();
        }
    }
}
