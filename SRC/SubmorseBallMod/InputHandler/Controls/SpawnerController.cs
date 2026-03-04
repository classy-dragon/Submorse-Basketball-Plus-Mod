using UnityEngine.InputSystem;
using GlobalConfig;
using UnityEngine;

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
    public static class SpawnerController
    {
        private static GameObject BallCopy;
        private static GameObject BallGhost;
        public static bool SpawnerActive = false;
        public static bool SpawnerDelateMode = false;
        public static Color SpawnColor = new Color(0.5f, 0.5f, 0.9f, 0.3f);
        public static Color DelateColor = new Color(1f, 0.5f, 0.5f, 0.3f);
        public static float SpawnerSize = 0.5f;
        private static void SetGhostBallColor(bool DelateMode)
        {
            if (BallGhost && BallGhost.TryGetComponent<Renderer>(out Renderer RD))
            {
                if (DelateMode)
                {
                    RD.material.color = DelateColor;
                    RD.material.SetColor("_Color",DelateColor);
                } else
                {
                    RD.material.color = SpawnColor;
                    RD.material.SetColor("_Color", SpawnColor);
                }
            }
        }
        public static void Setup()
        {
            if (!BallGhost)
            {
                BallGhost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                BallGhost.transform.SetParent(Camera.main.transform);
                BallGhost.transform.localPosition = new Vector3(0, 0, 1.5f);
                BallGhost.transform.localScale = new Vector3(SpawnerSize, SpawnerSize, SpawnerSize);
                var renderer = BallGhost.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("UI/Default");
                SetGhostBallColor(false);
                BallGhost.SetActive(false);
                BallGhost.GetComponent<Collider>().enabled = false;
            }
            if (!BallCopy)
            {
                BallCopy = GameObject.Instantiate(GameObject.FindObjectOfType<BasketBall>().gameObject);
                BallCopy.SetActive(false);
                BallCopy.transform.position = new Vector3(9999, 9999, 9999);
            }
        }

        public static void UPTEditor()
        {
            if (!BallGhost) Setup();
            BallGhost.SetActive(SpawnerActive);
        }
        public static void SpawnBall()
        {
            GameObject NewBall = GameObject.Instantiate(BallCopy);
            NewBall.transform.position = BallGhost.transform.position;
            NewBall.SetActive(true);
        }
        public static void DelateBall()
        {
            Collider[] Colliders = Physics.OverlapSphere(BallGhost.transform.position, SpawnerSize);
            foreach (var Object in Colliders)
            {
                if (Object.gameObject.GetComponent<BasketBall>())
                {
                    GameObject.Destroy(Object.gameObject);
                }
            }
        }
        public static void Run()
        {
            if (ModConfig.HoldingObject) return;
            if (Inputmap.DelateMode())
            {
                SpawnerDelateMode = !SpawnerDelateMode;
                SetGhostBallColor(SpawnerDelateMode);
                return;
            }
            else if (Inputmap.ToggleSpawner())
            {
                SpawnerActive = !SpawnerActive;
                UPTEditor();
                return;
            }
            if (Inputmap.Spawn() && SpawnerActive)
            {
                if (SpawnerDelateMode)
                {
                    DelateBall();
                }
                else
                {
                    SpawnBall();
                }
            }
        }
    }
}
