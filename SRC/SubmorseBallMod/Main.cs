using BepInEx;
using ControllerMapClass;
using GlobalConfig;
using RespawnTriggerClass;

namespace BasketballPlus
{
    [BepInPlugin("com.classy.submorse", "Basketball+", "1.1.0")]
    public class ballmodmain : BaseUnityPlugin
    {
        void Awake()
        {
            Startup.INIT(gameObject);
        }
        void Update()
        {
            ControllerMap.RunInputs();
        }
    }
}