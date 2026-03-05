using UnityEngine.InputSystem;
using GlobalConfig;
using UnityEngine;
using TweaksHelper;
using BLogger = BepInEx.Logging.Logger;
using BepInEx.Logging;
using System.CodeDom;

namespace SpawnerControllerClass
{
    public class Inputmap
    {
        public static bool ToggleSpawner()
        {
            return Keyboard.current.mKey.wasPressedThisFrame;
        }
        public static bool Spawn()
        {
            return Keyboard.current.enterKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame;
        }
        public static bool DelateMode()
        {
            return Keyboard.current.nKey.wasPressedThisFrame || Mouse.current.backButton.wasPressedThisFrame;
        }
    }
    public class BasketballEditor
    {
        GameObject GhostHelper;
        GameObject BasketballCopy;
        bool Active = false;
        bool Placemode = true;
        ManualLogSource EditorLog = BLogger.CreateLogSource("Basketball+ Editor");
        private class GhostColors
        {
            public static Color Normal = new Color(0.5f, 0.5f, 0.9f, 0.3f);
            public static Color Delate = new Color(1f, 0.5f, 0.5f, 0.3f);
        }
        public bool TryINIT(float SpawnerSize)
        {
            if (!GhostHelper || !BasketballCopy)
            {
                return Init(SpawnerSize);
            }
            return false;
        }
        public bool Init(float SpawnerSize)
        {
            if (!GhostHelper)
            {
                if (Camera.main)
                {
                    GhostHelper = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    var renderer = GhostHelper.GetComponent<MeshRenderer>();
                    renderer.material.shader = Shader.Find("UI/Default");
                    GhostHelper.transform.SetParent(Camera.main.transform);
                    GhostHelper.transform.localPosition = new Vector3(0, 0, 1.5f);
                    GhostHelper.transform.localScale = new Vector3(SpawnerSize, SpawnerSize, SpawnerSize);
                    GhostHelper.GetComponent<Collider>().enabled = false;
                    Helper.SetColor(GhostHelper, GhostColors.Normal);
                    GhostHelper.SetActive(false);
                }
            }
            if (!BasketballCopy)
            {
                BasketBall TargetBasketballCOMP = GameObject.FindObjectOfType<BasketBall>();
                if (TargetBasketballCOMP)
                {
                    BasketballCopy = GameObject.Instantiate(TargetBasketballCOMP.gameObject);
                    BasketballCopy.transform.position = new Vector3(9999, 9999, 9999);
                    BasketballCopy.SetActive(false);
                    EditorLog.LogMessage("INIT Basketball Object Found!");
                }
                //else
                //{
                //    EditorLog.LogWarning("Failed INIT Missing Basketball Object.");
                //}
            }
            return (BasketballCopy && GhostHelper);
        }
        private void Spawn(GameObject LocationTarget)
        {
            if (!Active) return;
            GameObject NewBasketball = GameObject.Instantiate(BasketballCopy).gameObject;
            NewBasketball.transform.SetPositionAndRotation(LocationTarget.transform.position, LocationTarget.transform.rotation);
            NewBasketball.SetActive(true);
        }
        private void Delate(GameObject LocationTarget)
        {
            if (!Active) return;
            Collider[] Colliders = Physics.OverlapSphere(LocationTarget.transform.position, LocationTarget.transform.localScale.y);
            foreach (var Object in Colliders)
            {
                if (Object.gameObject.GetComponent<BasketBall>())
                {
                    GameObject.Destroy(Object.gameObject);
                }
            }
        }
        public void Activate()
        {
            if (!Active) return;
            if (Placemode) Spawn(GhostHelper); else Delate(GhostHelper);
        }
        public void SetPlaceMode(bool NewState)
        {
            Placemode = NewState;
            if (NewState) Helper.SetColor(GhostHelper, GhostColors.Normal); else Helper.SetColor(GhostHelper, GhostColors.Delate);
        }
        public void TogglePlaceMode()
        {
            SetPlaceMode(!Placemode);
        }
        public void SetActive(bool NewState)
        {
            Active = NewState;
            GhostHelper.SetActive(NewState);
        }
        public void ToggleActive()
        {
            SetActive(!Active);
        }
    }
    public static class SpawnerController
    {
        public static float SpawnerSize = 0.5f;
        public static BasketballEditor Editor = new BasketballEditor();
        private static bool Hooked = false;
        public static void Init()
        {
            Editor.TryINIT(SpawnerSize);
        }
        public static void Run()
        {
            if (ModConfig.HoldingObject)
            {
                Editor.SetActive(false);
                return;
            }
            if (Inputmap.DelateMode())
            {
                Editor.TogglePlaceMode();
                return;
            }
            else if (Inputmap.ToggleSpawner())
            {
                Editor.ToggleActive();
                return;
            }
            if (Inputmap.Spawn())
            {
                Editor.Activate();
                return;
            }
            if (!Hooked)
            {
                Hooked = Editor.TryINIT(SpawnerSize);
                BepInEx.Logging.Logger.CreateLogSource("e").LogInfo("not hooked");
            }
        }
    }
}
