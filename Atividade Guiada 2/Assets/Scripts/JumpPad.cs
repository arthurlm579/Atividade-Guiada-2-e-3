using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float forcaDoPulo = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.DarImpulsoVertical(forcaDoPulo);
            }
        }
    }
}