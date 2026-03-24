        using UnityEngine;

        public class Checkpoint : MonoBehaviour
        {
            private void OnTriggerEnter(Collider other)
            {
                if (other.CompareTag("Player"))
                {
                    PlayerMovement pm = other.GetComponent<PlayerMovement>();
                    if (pm != null)
                    {
                        // Avisa ao Player que este é o novo lugar de renascer
                        pm.DefinirCheckpoint(transform.position + Vector3.up);
                        Debug.Log("Checkpoint Salvo!");
                    }
                }
            }
        }