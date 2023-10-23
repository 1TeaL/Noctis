using NoctisMod.SkillStates;
using System.Collections.Generic;
using System;
using NoctisMod.SkillStates.BaseStates;
using NoctisMod.SkillStates;

namespace NoctisMod.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        internal static void RegisterStates()
        {
            entityStates.Add(typeof(BaseMeleeAttack));

            //Utility
            entityStates.Add(typeof(Dodge));

            //Sword
            entityStates.Add(typeof(SwordCombo));
            entityStates.Add(typeof(SwordNeutral));
            entityStates.Add(typeof(SwordForward));
            entityStates.Add(typeof(SwordBackward));
            entityStates.Add(typeof(SwordAerial));
            entityStates.Add(typeof(SwordSwapNeutral));
            entityStates.Add(typeof(SwordSwapNeutral2));
            entityStates.Add(typeof(SwordSwapForward));
            entityStates.Add(typeof(SwordSwapBackward));
            entityStates.Add(typeof(SwordSwapAerial));
            entityStates.Add(typeof(SwordSwapAerial2));

            //Greatsword

            //Polearm
        }
    }
}