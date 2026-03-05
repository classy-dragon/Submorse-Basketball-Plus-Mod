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
            if (Keyboard.current==null || Mouse.current==null) return;
            if (PuzzleFocus.Focused)
            {
                SpawnerController.Editor.SetActive(false);
                if (ModConfig.HoldingObject)
                {
                    BallController.CastToss();
                }
                return;
            }
            BallController.Run();
            ThrustController.Run();
            SpawnerController.Run();
        }
    }
}