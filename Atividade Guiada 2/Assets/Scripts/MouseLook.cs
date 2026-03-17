using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o objeto 'Player' para cá.")]
    [SerializeField] private Transform corpoDoJogador;

    [Header("Configurações")]
    // Mudamos para public para o Slider do Menu de Pausa funcionar
    public float sensibilidade = 150f;

    [SerializeField] private float limiteVerticalMin = -90f;
    [SerializeField] private float limiteVerticalMax = 90f;

    private float rotacaoX = 0f;

    void Start()
    {
        // Garante que o mouse comece travado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (corpoDoJogador == null)
        {
            Debug.LogError("MouseLook: Falta o Transform do Jogador!");
        }
    }

    void Update()
    {
        // Se o jogo estiver em pausa (Time.timeScale = 0), não gira a câmera
        if (Mathf.Approximately(Time.timeScale, 0f)) return;

        MoverCamera();
    }

    private void MoverCamera()
    {
        // Usamos Time.unscaledDeltaTime para a sensibilidade ser constante
        float mouseX = Input.GetAxis("Mouse X") * sensibilidade * Time.unscaledDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidade * Time.unscaledDeltaTime;

        // Eixo Vertical (Olhar para cima e baixo)
        rotacaoX -= mouseY;
        rotacaoX = Mathf.Clamp(rotacaoX, limiteVerticalMin, limiteVerticalMax);
        transform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);

        // Eixo Horizontal (Girar o corpo do personagem)
        if (corpoDoJogador != null)
        {
            corpoDoJogador.Rotate(Vector3.up * mouseX);
        }
    }
}