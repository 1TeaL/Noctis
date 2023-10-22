using RoR2;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;
using System;
using Rewired;

namespace NoctisMod.Modules
{
    public static class Buffs
    {
        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static BuffDef dodgeBuff;

        internal static void RegisterBuffs()
        {
            dodgeBuff = Buffs.AddNewBuff($"Dodge armor buff", Assets.shieldBuffIcon, Color.white, false, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Buffs.buffDefs.Add(buffDef);

            return buffDef;
        }

    }
}
