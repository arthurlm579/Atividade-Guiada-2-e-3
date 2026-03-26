using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Configurações do Item")]
    public int valor = 1;
    public AudioClip som;
    public GameObject efeitoVisual;

    [Header("Efeito de Movimento")]
    public float velocidadeGiro = 100f;
    public float amplitude = 0.5f; // O quão alto ela vai
    public float frequencia = 2f;  // A velocidade da subida/descida
    private Vector3 posicaoInicial;

    void Start()
    {
        // Salva a posição inicial para a flutuação
        posicaoInicial = transform.position;
    }

    void Update()
    {
        // 1. Faz a moeda girar
        transform.Rotate(Vector3.up * velocidadeGiro * Time.deltaTime);

        // 2. Faz a moeda flutuar usando uma onda de Seno
        float novoY = posicaoInicial.y + Mathf.Sin(Time.time * frequencia) * amplitude;
        transform.position = new Vector3(posicaoInicial.x, novoY, posicaoInicial.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem colidiu foi o Player
        if (other.CompareTag("Player"))
        {
            // Toca o som, se houver
            if (som)
                AudioSource.PlayClipAtPoint(som, transform.position);

            // Cria o efeito visual, se houver
            if (efeitoVisual)
                Instantiate(efeitoVisual, transform.position, Quaternion.identity);

            // 1. Tenta avisar o script do Player para contar a vitória
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.ColetarMoeda();
            }

            // 2. Tenta avisar a HUD (estático)
            // Nota: Se você ainda não criou o script PontuacaoHUD, 
            // comente a linha abaixo com // para não dar erro.
            // PontuacaoHUD.AdicionarPontos(valor);

            // Destrói a moeda
            Destroy(gameObject);
        }
    }
}