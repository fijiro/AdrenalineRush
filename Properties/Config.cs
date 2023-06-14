using BepInEx.Configuration;

namespace Adrenaline.Configs
{
    internal class AdrenalineConfig
    {
        public static ConfigEntry<bool> EnableMod { get; private set; }
		public static ConfigEntry<bool> EnableLog { get; private set; }
		public static ConfigEntry<int> AdrenalineDuration { get; private set; }
		public static ConfigEntry<int> AdrenalineCooldown { get; private set; }
		public static ConfigEntry<bool> DisableDrawbacks { get; private set; }

		public static void Init(ConfigFile Config)
        {
            string configSettings = "General Settings";

            EnableMod = Config.Bind(configSettings, "Enable Mod", true,
                new ConfigDescription("Enable or disable the mod",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 100 }));

			EnableLog = Config.Bind(configSettings, "Enable Log", false,
				new ConfigDescription("Enable or disable logging",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = true, Order = 75 }));

			AdrenalineDuration = Config.Bind(configSettings, "Adrenaline Duration", 20,
                new ConfigDescription("Tunnelvision duration is half of this value",
                new AcceptableValueRange<int>(0, 120),
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 50 }));

			AdrenalineCooldown = Config.Bind(configSettings, "Adrenaline Cooldown", 60,
				new ConfigDescription("Cooldown starts after the painkiller effect wears off",
				new AcceptableValueRange<int>(0, 120),
				new ConfigurationManagerAttributes { IsAdvanced = false, Order = 00 }));

			DisableDrawbacks = Config.Bind(configSettings, "Disable Drawbacks", false,
				new ConfigDescription("Disable negative effects caused by adrenaline rush",
				null,
				new ConfigurationManagerAttributes { IsAdvanced = false, Order = -50 }));
		}
	}
}