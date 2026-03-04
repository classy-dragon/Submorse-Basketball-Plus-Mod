using GlobalConfig;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TweaksHelper
{
    public static class Helper
    {
        public static void SetGravity(GameObject Ball, bool NewState)
        {
            if (!Camera.main) return;
            if (!NewState) Ball.transform.SetParent(Camera.main.transform); else Ball.transform.SetParent(null);
            if (Ball.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.useGravity = NewState;
                rb.isKinematic = !NewState;
                if (Ball.TryGetComponent<Collider>(out Collider hitbox))
                {
                    hitbox.enabled = NewState;
                }
            }
        }
        public static LineRenderer CreateLine(GameObject Parent)
        {
            LineRenderer line;
            line = Parent.AddComponent<LineRenderer>();
            line.startWidth = 0.05f;
            line.endWidth = 0.02f;
            line.positionCount = 0;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.white;
            line.endColor = Color.gray;
            return line;
        }
        public static GameObject GetCursorTarget()
        {
            Camera MainCam = Camera.main;
            if (MainCam == null) return null;
            Ray RayInfo = MainCam.ScreenPointToRay(Mouse.current.position.ReadValue()); // new Vector3(0.5f,0.5f,0)
            if (Physics.Raycast(RayInfo, out RaycastHit CastInfo,3f))
            {
                return CastInfo.collider.gameObject;
            }
            return null;
        }
        public static void DrawProjection(LineRenderer line, GameObject TargetObject)
        {
            if (TargetObject == null)
            {
                line.positionCount = 0;
                return;
            }

            int segmentCount = 30;
            float timeStep = 0.05f;
            line.positionCount = segmentCount;

            Vector3 lastPos = TargetObject.transform.position;
            Vector3 startPos = TargetObject.transform.position;

            float simulatedThrust = ModConfig.CurrentThrust;
            Vector3 velocity = (Camera.main.transform.forward + Vector3.up).normalized * simulatedThrust;

            if (TargetObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                velocity /= rb.mass;
            }

            for (int i = 0; i < segmentCount; i++)
            {
                float t = i * timeStep;
                Vector3 nextPos = TargetObject.transform.position + (velocity * t) + (0.5f * Physics.gravity * t * t);
                if (Physics.Linecast(lastPos, nextPos, out RaycastHit hit))
                {
                    line.SetPosition(i, hit.point);
                    line.positionCount = i + 1;
                    break;
                }

                line.SetPosition(i, nextPos);
                lastPos = nextPos;
            }
        }
    }
}