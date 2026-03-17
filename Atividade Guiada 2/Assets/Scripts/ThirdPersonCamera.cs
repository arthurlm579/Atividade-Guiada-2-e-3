using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Configurações de Alvo")]
    public Transform alvo;
    public Vector3 offsetFixo = new Vector3(0, 1.2f, 0); // Altura do "olhar" sobre o alvo

    [Header("Movimento")]
    public float sensibilidade = 150f;
    public float suavizacao = 10f;

    [Header("Zoom")]
    public float distanciaAtual = 4f;
    public float distanciaMin = 2f;
    public float distanciaMax = 8f;
    public float velocidadeZoom = 5f;

    [Header("Colisão")]
    public LayerMask camadasObstaculos;
    public float raioColisao = 0.2f;

    private float yaw;   // Rotação Horizontal
    private float pitch; // Rotação Vertical
    private float distanciaDesejada;

    void Start()
    {
        distanciaDesejada = distanciaAtual;

        // Inicializa as rotações baseadas na rotação atual da câmera
        Vector3 rot = transform.eulerAngles;
        yaw = rot.y;
        pitch = rot.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // LateUpdate é melhor para câmeras para evitar tremores após o movimento do Player
    void LateUpdate()
    {
        if (!alvo) return;

        // 1. Entrada do Mouse (Rotação)
        yaw += Input.GetAxis("Mouse X") * sensibilidade * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensibilidade * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -20f, 60f); // Limita para não dar volta na cabeça

        // 2. Entrada do Scroll (Zoom)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distanciaDesejada = Mathf.Clamp(distanciaDesejada - scroll * velocidadeZoom, distanciaMin, distanciaMax);

        // 3. Calcular Rotação e Posição Teórica
        Quaternion rotacao = Quaternion.Euler(pitch, yaw, 0);

        // Ponto de foco (ex: cabeça do player)
        Vector3 pontoFoco = alvo.position + offsetFixo;

        // Posição ideal da câmera (antes da colisão)
        Vector3 direcaoCâmera = rotacao * Vector3.back;
        Vector3 posicaoDesejada = pontoFoco + direcaoCâmera * distanciaDesejada;

        // 4. Checagem de Colisão (Anti-Clipping)
        // Faz um SphereCast do player em direção à câmera para ver se há paredes no caminho
        RaycastHit hit;
        float distanciaFinal = distanciaDesejada;

        if (Physics.SphereCast(pontoFoco, raioColisao, direcaoCâmera, out hit, distanciaDesejada, camadasObstaculos))
        {
            // Se bater em algo, a distância é reduzida para o ponto do impacto
            distanciaFinal = Mathf.Clamp(hit.distance - 0.1f, 0.5f, distanciaDesejada);
        }

        // 5. Aplicar Posição e Rotação Suavizada
        Vector3 posicaoFinal = pontoFoco + direcaoCâmera * distanciaFinal;

        transform.position = Vector3.Lerp(transform.position, posicaoFinal, Time.deltaTime * suavizacao);
        transform.LookAt(pontoFoco);
    }
}