using UnityEngine.InputSystem;
using GlobalConfig;
using UnityEngine;
using TweaksHelper;
using BLogger = BepInEx.Logging.Logger;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace SpawnerControllerClass
{
    public class Inputmap
    {
        public static bool ToggleSpawner() => Keyboard.current.mKey.wasPressedThisFrame;
        public static bool Spawn() => Keyboard.current.enterKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame;
        public static bool DeleteMode() => Keyboard.current.nKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame;
        public static bool ClearAll() => Keyboard.current.leftCtrlKey.IsPressed() && Keyboard.current.nKey.wasPressedThisFrame;
    }
    public class BasketballEditor
    {
        float InitSizeCache = 1f;
        List<GameObject> SpawnerCache = new List<GameObject>();
        bool SpawnedActiveCache;
        bool Active = false;
        bool Placemode = true;
        ManualLogSource EditorLog = BLogger.CreateLogSource("Basketball+ Editor");
        GameObject GhostHelper;
        GameObject BasketballCopy;
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
            InitSizeCache = SpawnerSize;
            if (!GhostHelper)
            {
                if (Camera.main)
                {
                    GhostHelper = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GameObject.DontDestroyOnLoad(GhostHelper);
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
                    BasketballCopy.name = "Basketball+ : Main Copy";
                    BasketballCopy.SetActive(false);
                    BasketballCopy.transform.SetParent(ModConfig.GameRoot ? ModConfig.GameRoot.transform : null);
                    EditorLog.LogMessage("INIT Basketball Object Found!");
                }
                else
                {
                    EditorLog.LogWarning("Failed INIT Missing Basketball Object.");
                }
            }
            return (BasketballCopy && GhostHelper);
        }
        private void Spawn(GameObject LocationTarget)
        {
            if (!Active) return;
            if (!BasketballCopy) if (!TryINIT(InitSizeCache)) return;
            GameObject NewBasketball = GameObject.Instantiate(BasketballCopy, ModConfig.SpawnerRoot ? ModConfig.SpawnerRoot.transform : null).gameObject;
            NewBasketball.name = "Basketball";
            NewBasketball.transform.SetPositionAndRotation(LocationTarget.transform.position, LocationTarget.transform.rotation);
            NewBasketball.SetActive(true);
            SpawnerCache.Add(NewBasketball);
        }
        private void Delete(GameObject LocationTarget)
        {
            if (!Active) return;
            Collider[] Colliders = Physics.OverlapSphere(LocationTarget.transform.position, LocationTarget.transform.localScale.y);
            foreach (var Object in Colliders)
            {
                if (Object.name == "Basketball" || Object.gameObject.GetComponent<BasketBall>())
                {
                    if (SpawnerCache.Contains(Object.gameObject)) SpawnerCache.Remove(Object.gameObject);
                    GameObject.Destroy(Object.gameObject);
                }
            }
        }
        public void SetSpawnedActive(bool NewState)
        {
            if (NewState != SpawnedActiveCache)
            {
                SpawnerCache.RemoveAll(item => item == null);
                foreach (GameObject Object in SpawnerCache)
                {
                    if (Object) Object.SetActive(NewState);
                }
                SpawnedActiveCache = NewState;
            }
        }
        public void ClearSpawned()
        {
            foreach (GameObject Object in SpawnerCache)
            {
                if (Object) GameObject.Destroy(Object);
            }
            SpawnerCache.Clear();
            EditorLog.LogInfo("Cleared Spawned Objects.");
        }
        public void Activate()
        {
            SpawnerCache.RemoveAll(item => item == null);
            if (!Active) return;
            if (!GhostHelper) if (!TryINIT(InitSizeCache)) return;
            if (Placemode) Spawn(GhostHelper); else Delete(GhostHelper);
        }
        public bool ReadActive() { return Active; }
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
            if (GhostHelper)
            {
                GhostHelper.SetActive(NewState);
            }
            else
            {
                if (Active) TryINIT(InitSizeCache);
            }
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
        public static void OnInit()
        {
            if (!Hooked)
            {
                Hooked = Editor.TryINIT(SpawnerSize);
            }
        }
        public static void Run()
        {
            if (ModConfig.HoldingObject)
            {
                Editor.SetActive(false);
                return;
            }
            if (Inputmap.ToggleSpawner())
            {
                Editor.ToggleActive();
                return;
            }
            if (Editor.ReadActive())
            {
                if (Inputmap.ClearAll())
                {
                    Editor.ClearSpawned();
                    return;
                }
                if (Inputmap.DeleteMode())
                {
                    Editor.TogglePlaceMode();
                    return;
                }
                if (Inputmap.Spawn())
                {
                    Editor.Activate();
                    return;
                }
            }

        }
    }
}