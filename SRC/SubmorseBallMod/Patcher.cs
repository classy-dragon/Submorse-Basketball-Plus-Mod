using FMODUnity;
using GlobalConfig;
using HarmonyLib;
using Submorse.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PatcherClass
{
    public static class Patcher
    {
        // INIT Patcher
        [HarmonyPatch(typeof(SubAmbience), "Start")]
        public class InitPatch
        {
            [HarmonyPrefix]
            static void GameInit(SubAmbience __instance)
            {
                Startup.Ready();
                // Studio Audio Patcher
                if (Camera.main)
                {
                    Component Listoner = Camera.main.gameObject.GetComponent<StudioListener>();
                    if (!Listoner)
                    {
                        Camera.main.gameObject.AddComponent<StudioListener>();
                    }
                }
            }
        }
        // Player Jump Patch
        [HarmonyPatch(typeof(PlayerMovement), "Update")]
        public class PlayerJumpPatch
        {
            private static float JumpF = 3.5f;
            [HarmonyPostfix]
            static void HandleJump(PlayerMovement __instance)
            {
                var Traverser = Traverse.Create(__instance);
                var Controller = Traverser.Field("controller").GetValue<CharacterController>();
                if (Keyboard.current.spaceKey.IsPressed() && __instance.IsGrounded())
                {
                    Traverser.Field("gravity").SetValue(JumpF);
                    Controller.Move(Vector3.up * 0.01f);
                }
            }
        }
        // Basketball Thrust Controller
        [HarmonyPatch(typeof(BasketBall), "OnPlayerInteract")]
        public class BallThrustPatch
        {
            [HarmonyPrefix]
            static void SetThrustPower(BasketBall __instance)
            {
                Traverse.Create(__instance).Field("thrust").SetValue(ModConfig.CurrentThrust);
            }
        }
        // Main Patch
        public static void Patch()
        {
            Harmony HarmPatcher = new Harmony("com.submorse.basketballplus");
            HarmPatcher.PatchAll();
        }
    }
}