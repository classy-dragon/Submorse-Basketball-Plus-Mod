using BallThrustControllerClass;
using BallControllerClass;
using SpawnerControllerClass;
using Submorse;
using GlobalConfig;

namespace ControllerMapClass
{
    public static class ControllerMap
    {
        public static void RunInputs()
        {
            if (PuzzleFocus.Focused)
            {
                SpawnerController.SpawnerActive = false;
                SpawnerController.UPTEditor();
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