using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using UnityEngine;

namespace NoctisMod.Modules
{
    public static class Config
    {
        public static ConfigEntry<bool> allowVoice;

        public static void ReadConfig()
        {
            allowVoice = NoctisPlugin.instance.Config.Bind("General", "Allow voice", true, "Allow voice lines of Noctis.");

        }

        // this helper automatically makes config entries for disabling survivors
        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return NoctisPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this character"));
        }

        internal static ConfigEntry<bool> EnemyEnableConfig(string characterName)
        {
            return NoctisPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this enemy"));
        }

        public static void SetupRiskOfOptions()
        {
            //Risk of Options intialization
            ModSettingsManager.AddOption(new CheckBoxOption(
                allowVoice));
            ModSettingsManager.SetModDescription("Noctis Mod");
            Sprite icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Noctis");
            ModSettingsManager.SetModIcon(icon);

        }
    }
}
