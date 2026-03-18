using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public Transform pontoA, pontoB;
    public float velocidade = 3f;
    private Vector3 destinoAtual;
    private Vector3 posicaoAnterior;
    private PlayerMovement player;

    void Start()
    {
        destinoAtual = pontoB.position;
        posicaoAnterior = transform.position;
    }

    void FixedUpdate()
    {
        // Movimentação da plataforma
        transform.position = Vector3.MoveTowards(transform.position, destinoAtual, velocidade * Time.fixedDeltaTime);

        // Cálculo do deslocamento deste frame
        Vector3 movimentoDesteFrame = transform.position - posicaoAnterior;

        // Se o player estiver sobre a plataforma, aplica o movimento nele
        if (player != null)
        {
            player.AdicionarMovimentoExterno(movimentoDesteFrame);
        }

        posicaoAnterior = transform.position;

        // Alternância de destino ao chegar nos pontos
        if (Vector3.Distance(transform.position, destinoAtual) < 0.01f)
        {
            destinoAtual = (destinoAtual == pontoA.position) ? pontoB.position : pontoA.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerMovement>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}