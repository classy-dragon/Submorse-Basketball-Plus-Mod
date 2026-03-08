using BallThrustControllerClass;
using BallControllerClass;
using SpawnerControllerClass;
using Submorse;
using GlobalConfig;
using UnityEngine.InputSystem;

namespace ControllerMapClass
{
    public static class ControllerMap
    {
        public static void HookOnReadyInputs()
        {
            Startup.OnReady += () => {
                SpawnerController.OnInit();
            };
        }
        public static void RunInputs()
        {
            if (Keyboard.current == null || Mouse.current == null) return;
            if (!ModConfig.Active)
            {
                SpawnerController.Editor.SetSpawnedActive(false);
                SpawnerController.Editor.SetActive(false);
                if (ModConfig.HoldingObject)
                {
                    BallController.CastToss();
                }
                return;
            } else
            {
                SpawnerController.Editor.SetSpawnedActive(true);
            }
            if (SpawnerController.Editor.ReadActive()) { SpawnerController.Run(); return; }
            BallController.Run();
            ThrustController.Run();
            SpawnerController.Run();
        }
    }
}