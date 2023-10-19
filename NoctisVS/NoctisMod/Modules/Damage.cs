using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoctisMod.Modules
{
    public static class Damage
    {
        internal static DamageAPI.ModdedDamageType shiggyDecay;

        internal static void SetupModdedDamage()
        {
            shiggyDecay = DamageAPI.ReserveDamageType();
        }
    }
}
