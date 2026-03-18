using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.Morrer(); // O player volta para o último checkpoint salvo
            }
        }
    }
}