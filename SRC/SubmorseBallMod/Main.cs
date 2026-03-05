using BepInEx;
using ControllerMapClass;
using GlobalConfig;
using RespawnTriggerClass;

namespace BasketballPlusClass
{
    [BepInPlugin("com.classy.submorse", "Basketball+", "1.1.0")]
    public class BasketballPlusPlugin : BaseUnityPlugin
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