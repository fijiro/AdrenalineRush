using BepInEx.Configuration;

namespace Adrenaline.Configs
{
    internal class AdrenalineConfig
    {
        public static ConfigEntry<bool> EnableMod { get; private set; }
        public static ConfigEntry<float> AdrenalineDuration { get; private set; }

        public static void Init(ConfigFile Config)
        {
            string debugmode = "General Settings";
            EnableMod = Config.Bind(debugmode, "Mod Enabled", true,
                new ConfigDescription("Enable or disable the mod",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 100 }));

            AdrenalineDuration = Config.Bind(debugmode, "Adrenaline Duration", 30f,
                new ConfigDescription("Duration of the painkiller buff in seconds. Doesn't require restart",
                new AcceptableValueRange<float>(0f, 120f),
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 50 }));
        }
    }
}