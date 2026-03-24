using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [Header("Configurações de Rota")]
    public Transform[] pontos;
    public float velocidade = 3f;
    public float tempoDeEspera = 0.5f;

    private int indiceAtual = 0;
    private float cronometroEspera;
    private Vector3 posicaoAnterior;
    private PlayerMovement player;

    void Start()
    {
        if (pontos.Length > 0)
        {
            transform.position = pontos[0].position;
            posicaoAnterior = transform.position;
        }
    }

    void FixedUpdate()
    {
        if (pontos.Length < 2) return;

        MoverPlataforma();
        posicaoAnterior = transform.position;

        // SEGURANÇA: Se o player se afastar demais subitamente (Morte/Teleporte)
        // a plataforma para de enviar movimento para ele.
        if (player != null)
        {
            float distancia = Vector3.Distance(transform.position, player.transform.position);
            if (distancia > 5f) // Se estiver a mais de 5 metros, desconecta
            {
                player = null;
            }
        }
    }

    void MoverPlataforma()
    {
        Vector3 destino = pontos[indiceAtual].position;
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidade * Time.fixedDeltaTime);

        Vector3 movimentoDesteFrame = transform.position - posicaoAnterior;

        if (player != null)
        {
            player.AdicionarMovimentoExterno(movimentoDesteFrame);
        }

        if (Vector3.Distance(transform.position, destino) < 0.01f)
        {
            cronometroEspera += Time.fixedDeltaTime;
            if (cronometroEspera >= tempoDeEspera)
            {
                indiceAtual = (indiceAtual + 1) % pontos.Length;
                cronometroEspera = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) player = other.GetComponent<PlayerMovement>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) player = null;
    }

    // Desenha a linha da rota na aba Scene para facilitar sua vida
    private void OnDrawGizmos()
    {
        if (pontos == null || pontos.Length < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < pontos.Length; i++)
        {
            if (pontos[i] == null) continue;
            Gizmos.DrawSphere(pontos[i].position, 0.2f);
            int proximo = (i + 1) % pontos.Length;
            if (pontos[proximo] != null)
                Gizmos.DrawLine(pontos[i].position, pontos[proximo].position);
        }
    }
}