using UnityEngine.InputSystem;
using GlobalConfig;
using TweaksHelper;
using UnityEngine;

namespace BallThrustControllerClass
{
    public class Inputmap {
        public static bool Up() {
            return Keyboard.current.upArrowKey.wasPressedThisFrame || (Mouse.current.scroll.y.ReadValue()>0.1f);
        }
        public static bool Down()
        {
            return Keyboard.current.downArrowKey.wasPressedThisFrame || (Mouse.current.scroll.y.ReadValue() < -0.1f);
        }
        public static bool ToggleARC()
        {
            return Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.tKey.wasPressedThisFrame;
        }
    }
    public static class ThrustController
    {
        public static void Run()
        {
            if (!ModConfig.HoldingObject) return;
            if (Inputmap.ToggleARC()) {
                ModConfig.ShowARC = !ModConfig.ShowARC;
                if (!ModConfig.ShowARC)
                {
                    ModConfig.ARCLine.positionCount = 0;
                }
            }
            if (ModConfig.ShowARC && ModConfig.HoldingObject && ModConfig.ARCLine)
            {
                Helper.DrawProjection(ModConfig.ARCLine,ModConfig.HoldingObject.gameObject);
            }
            if (Inputmap.Up())
            {
                ModConfig.CurrentThrust += ModConfig.ThrustStep;
            } else if (Inputmap.Down())
            {
                ModConfig.CurrentThrust -= ModConfig.ThrustStep;
            }
            ModConfig.CurrentThrust = Mathf.Clamp(ModConfig.CurrentThrust,0.1f,ModConfig.MaxThrust);
        }
    }
}
