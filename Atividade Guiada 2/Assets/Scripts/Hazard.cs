using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Se o que entrou no sensor for o Player...
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Morrer(); // Chama a tela de derrota e pausa o jogo
            }
        }
    }
}