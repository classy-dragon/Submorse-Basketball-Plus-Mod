using BepInEx;
using ControllerMapClass;
using FMOD;
using GlobalConfig;
using RespawnTriggerClass;
using Steamworks;
using UnityEngine;

namespace BasketballPlusClass
{
    [BepInPlugin("com.classy.submorse", "Basketball+", "1.1.1")]
    public class BasketballPlusPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            GameObject Root = new GameObject("Basketball+");
            Root.transform.SetParent(gameObject.transform);
            Startup.INIT(Root);
            Logger.LogMessage("Basketball+ Loading.");
        }
        void Update()
        {
            ControllerMap.RunInputs();
        }
    }
}