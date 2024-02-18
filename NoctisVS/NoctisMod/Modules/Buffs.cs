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

        internal static BuffDef armorBuff;
        internal static BuffDef GSarmorBuff;
        internal static BuffDef vulnerabilityDebuff;
        internal static BuffDef counterBuff;
        internal static BuffDef armigerBuff;
        internal static BuffDef manaBuff;

        internal static void RegisterBuffs()
        {
            armorBuff = Buffs.AddNewBuff($"Armor buff", Assets.shieldBuffIcon, Color.white, false, false);
            GSarmorBuff = Buffs.AddNewBuff($"GS Armor buff", Assets.shieldBuffIcon, Color.cyan, false, false);
            vulnerabilityDebuff = Buffs.AddNewBuff($"Vulnerability debuff", Assets.deathMarkDebuffIcon, Color.white, true, true);
            counterBuff = Buffs.AddNewBuff($"Counter buff", Assets.shieldBuffIcon, Color.black, false, false);

            armigerBuff = Buffs.AddNewBuff($"Armiger buff", Assets.ruinDebuffIcon, Color.white, false, false);
            manaBuff = Buffs.AddNewBuff($"Mana Regen buff", Assets.jumpBuffIcon, Color.cyan, false, false);
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
