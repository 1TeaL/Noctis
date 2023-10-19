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


            //base skills
            entityStates.Add(typeof(SwordCombo));
        }
    }
}