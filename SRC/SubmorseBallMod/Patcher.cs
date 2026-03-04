using GlobalConfig;
using HarmonyLib;
using Submorse.Player;
using SubmorseBallMod;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PatcherClass
{
    public static class Patcher
    {
        [HarmonyPatch(typeof(PlayerMovement), "Update")]
        public class PlayerJumpPatch
        {
            private static float JumpF = 3.5f;
            [HarmonyPrefix]
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
        [HarmonyPatch(typeof(BasketBall), "OnPlayerInteract")]
        public class BallThrustPatch
        {
            [HarmonyPrefix]
            static void SetThrustPower(BasketBall __instance)
            {
                Traverse.Create(__instance).Field("thrust").SetValue(ModConfig.CurrentThrust);
            }
        }
        public static void Patch()
        {
            Harmony.CreateAndPatchAll(typeof(BallThrustPatch));
            Harmony.CreateAndPatchAll(typeof(PlayerJumpPatch));
        }
    }
}