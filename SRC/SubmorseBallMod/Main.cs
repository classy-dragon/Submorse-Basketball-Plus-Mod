using BepInEx;
using ControllerMapClass;
using GlobalConfig;

namespace SubmorseBallMod
{
    [BepInPlugin("com.classy.submorse", "Basketball+", "1.0.0")]
    public class ballmodmain : BaseUnityPlugin
    {
        void Awake()
        {
            ModConfig.INIT(gameObject);
        }
        void Update()
        {
            ControllerMap.RunInputs();
        }
    }
}
