using UnityEngine;

namespace TheCleansing.Lobby
{
    public class PlayerSpawnPoint : MonoBehaviour                  
    {
        private void Awake()
        {
            PlayerSpawnSystem.AddSpawnPoint(transform);             //adds spawn obejct to list
        }

        private void OnDestroy()
        {
            PlayerSpawnSystem.RemoveSpawnPoint(transform);          //removes spawn object from list
        }

        private void OnDrawGizmos()                                 //debbugging tool, helps identify player object and the direction it's facing
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
        }
    }
}