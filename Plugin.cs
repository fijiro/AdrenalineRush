using BepInEx;
using Adrenaline.Configs;

namespace Adrenaline
{
    //todo: voiceline when adrenaline activates, cooldown?
    [BepInPlugin("com.jiro.adrenalinerush", "AdrenalineRush", "1.2.0")]
    public class AdrenalinePlugin : BaseUnityPlugin
    {
        void Awake()
        {
            AdrenalineConfig.Init(Config);
            new AdrenalinePatch().Enable();

        }
    }
}
