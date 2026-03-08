using BallControllerClass;
using GlobalConfig;
using UnityEngine;
namespace RespawnTriggerClass
{
    public class RespawnTrigger : MonoBehaviour
    {
        Vector3 TriggerSize = new Vector3(300, 5, 300);
        Vector3 WorldRespawn = new Vector3(0,2,0);
        GameObject Trigger;
        void Awake()
        {
            Trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Trigger.name = "Respawn-Trigger";
            Trigger.transform.SetParent(transform);
            Trigger.transform.localScale = TriggerSize;
            Trigger.transform.position = new Vector3(0, -20, 0);
            Trigger.GetComponent<Collider>().enabled = false;
            Trigger.GetComponent<Renderer>().enabled = false;
        }
        void FixedUpdate()
        {
            Collider[] hits = Physics.OverlapBox(Trigger.transform.position, TriggerSize/2f);
            foreach (Collider ColObject in hits)
            {
                if (ColObject.gameObject.TryGetComponent<BasketBall>(out BasketBall OBJBasketball))
                {
                    if (ModConfig.HoldingObject == OBJBasketball)
                    {
                        BallController.CastToss();
                        Camera.main.gameObject.transform.root.position = WorldRespawn; // Player out of bounds
                    }
                    OBJBasketball.gameObject.transform.position = WorldRespawn;
                    if (OBJBasketball.TryGetComponent<Rigidbody>(out Rigidbody RB))
                    {
                        RB.velocity = Vector3.zero;
                        RB.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
    }
}