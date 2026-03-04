using PatcherClass;
using TweaksHelper;
using UnityEngine;

namespace GlobalConfig
{
    public static class ModConfig
    {
        public static GameObject Parnet;
        public static BasketBall HoldingObject;
        public static float ThrustStep = 0.1f;
        public static float CurrentThrust = 0.7f;
        public static float MaxThrust = 5f;
        public static bool ShowARC = false;
        public static LineRenderer ARCLine;
        public static void INIT(GameObject gameObject)
        {
            Patcher.Patch();
            ModConfig.Parnet = gameObject;
            if (!ModConfig.ARCLine)
            {
                ModConfig.ARCLine = Helper.CreateLine(gameObject);
            }
        }
    }
}
