using FMODUnity;
using GlobalConfig;
using HarmonyLib;
using Submorse;
using Submorse.Player;
using System.CodeDom;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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
        // Load Active Patch
        [HarmonyPatch(typeof(SceneLoader), "LoadScene")]
        public class SceneINITPatch
        {
            [HarmonyPostfix]
            static void LoadScene(string sceneID) {
                ModConfig.Active = (sceneID == "Sub");
                if (ModConfig.SpawnerRoot)
                {
                    ModConfig.SpawnerRoot.SetActive(ModConfig.Active);
                }
            }
        }
        // ON Focus Patch
        [HarmonyPatch(typeof(PuzzleFocus), "Focus")]
        public class PuzzleOnFocus
        {
            [HarmonyPostfix]
            static void OnFocus()
            {
                ModConfig.Active = false;
            }
        }
        // OFF Focus Patch
        [HarmonyPatch(typeof(PuzzleFocus), "Unfocus")]
        public class PuzzleOffFocus
        {
            [HarmonyPrefix]
            static void OffFocus()
            {
                if (PuzzleFocus.Focused) return; // this gets ran even if there no Puzzle Focused, this patchs it.
                ModConfig.Active = true;
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
                if (Keyboard.current.spaceKey.IsPressed() && __instance.IsGrounded())
                {
                    var Traverser = Traverse.Create(__instance);
                    var Controller = Traverser.Field("controller").GetValue<CharacterController>();
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