using BepInEx.Configuration;

namespace Adrenaline.Configs
{
    internal class AdrenalineConfig
    {
        public static ConfigEntry<bool> EnableMod { get; private set; }
        public static ConfigEntry<float> AdrenalineDuration { get; private set; }
		public static ConfigEntry<float> AdrenalineCooldown { get; private set; }

		public static void Init(ConfigFile Config)
        {
            string configSettings = "General Settings";

            EnableMod = Config.Bind(configSettings, "Enable Mod", true,
                new ConfigDescription("Enable or disable the mod",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 100 }));

            AdrenalineDuration = Config.Bind(configSettings, "Adrenaline Duration", 20f,
                new ConfigDescription("Tunnelvision duration is half of this value",
                new AcceptableValueRange<float>(0f, 120f),
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 50 }));

			AdrenalineCooldown = Config.Bind(configSettings, "Adrenaline Cooldown", 60f,
				new ConfigDescription("Cooldown starts after the painkiller effect wears off",
				new AcceptableValueRange<float>(0f, 120f),
				new ConfigurationManagerAttributes { IsAdvanced = false, Order = 00 }));
		}

        public static float getCooldown()
        {
            return AdrenalineCooldown.Value;
        }
		public static float getDuration()
		{
			return AdrenalineDuration.Value;
		}
	}
}