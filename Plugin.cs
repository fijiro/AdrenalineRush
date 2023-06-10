using BepInEx;
using Adrenaline.Configs;

namespace Adrenaline
{

    //create BepInEx plugin and call the plugin
    [BepInPlugin("com.kobrakon.adrenaline", "Adrenaline", "1.0.0")]
    public class AdrenalinePlugin : BaseUnityPlugin
    {
        void Awake()
        {
            AdrenalineConfig.Init(Config);
            new AdrenalinePatch().Enable();
            Logger.LogInfo("Adrenaline Config value: " + AdrenalineConfig.AdrenalineDuration.Value.ToString());
            Logger.LogInfo("Adrenaline Default value: " + AdrenalineConfig.AdrenalineDuration.DefaultValue.ToString());
        }
    }
}
