using GlobalConfig;
using TweaksHelper;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BallControllerClass
{
    public class Inputmap
    {
        public static bool Grab()
        {
            return Keyboard.current.eKey.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame;
        }
        public static bool Throw()
        {
            return Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame;
        }
    }
    public static class BallController
    {
        public static void CastGrab()
        {
            GameObject Target = TweaksHelper.Helper.GetCursorTarget();
            if (Target != null && Target.TryGetComponent<BasketBall>(out BasketBall NewBall))
            {
                if (ModConfig.HoldingObject) Helper.SetGravity(ModConfig.HoldingObject.gameObject, true);
                Helper.SetGravity(NewBall.gameObject, false);
                NewBall.transform.localPosition = new Vector3(.5f, 0, 0.9f);
                ModConfig.HoldingObject = NewBall;
            }
        }
        public static void CastToss()
        {
            Helper.SetGravity(ModConfig.HoldingObject.gameObject, true);
            ModConfig.HoldingObject.OnPlayerInteract(Camera.main.gameObject);
            ModConfig.HoldingObject = null;
            ModConfig.ARCLine.positionCount = 0;
            return;
        }
        public static void Run()
        {
            if (!ModConfig.HoldingObject)
            {
                if (Inputmap.Grab())
                {
                    CastGrab();
                }
            }
            else
            {
                if (Inputmap.Throw())
                {
                    CastToss();   
                }
            }
        }
    }
}
