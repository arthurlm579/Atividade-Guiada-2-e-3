using UnityEngine;

public class Hazard : MonoBehaviour
{
    // Este método dispara quando algo entra no colisor do objeto (Lava, Espinho, etc)
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou tem a Tag "Player"
        if (other.CompareTag("Player"))
        {
            // Tenta pegar o componente de movimento do Player
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                // Chama a função Morrer que teletransporta e limpa as forças da plataforma
                player.Morrer();

                // Opcional: Você pode adicionar um som de morte ou efeito de partículas aqui
                Debug.Log("O Player tocou em um perigo!");
            }
        }
    }
}