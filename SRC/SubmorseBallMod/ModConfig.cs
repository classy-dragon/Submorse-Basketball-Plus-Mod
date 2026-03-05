using BallControllerClass;
using ControllerMapClass;
using PatcherClass;
using RespawnTriggerClass;
using System;
using TweaksHelper;
using UnityEngine;

namespace GlobalConfig
{
    public static class Startup
    {
        public static event Action OnReady;
        public static void Ready()
        {
            if (ModConfig.GameRoot)
            {
                if (!ModConfig.GameRoot.GetComponent<RespawnTrigger>()) ModConfig.GameRoot.AddComponent<RespawnTrigger>();
                if (!ModConfig.ARCLine) ModConfig.ARCLine = Helper.CreateLine(ModConfig.GameRoot);
            }
            OnReady?.Invoke();
        }
        public static void INIT(GameObject GameRoot)
        {
            ControllerMap.HookOnReadyInputs();
            ModConfig.GameRoot = GameRoot;
            Patcher.Patch(); // Calls to Ready when game is ready and loaded.
        }
    }
    public static class ModConfig
    {
        public static BasketBall HoldingObject;
        public static float ThrustStep = 0.1f;
        public static float CurrentThrust = 0.7f;
        public static float MaxThrust = 5f;
        public static bool ShowARC = false;
        public static LineRenderer ARCLine;
        public static GameObject GameRoot;
    }
}
