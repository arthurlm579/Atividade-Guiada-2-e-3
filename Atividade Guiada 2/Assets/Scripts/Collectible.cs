using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int valor = 1;
    public AudioClip som;
    public GameObject efeitoVisual;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem colidiu foi o Player
        if (other.CompareTag("Player"))
        {
            if (som)
                AudioSource.PlayClipAtPoint(som, transform.position);

            if (efeitoVisual)
                Instantiate(efeitoVisual, transform.position, Quaternion.identity);

            // Chama o método estático da HUD
            PontuacaoHUD.AdicionarPontos(valor);

            Destroy(gameObject);
        }
    }
}