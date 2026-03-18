using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;

    [Header("Configurações de Movimento")]
    public float velocidadeAndar = 6f;
    public float velocidadeCorrer = 10f;
    public float forcaPulo = 8f;
    public float gravidade = -25f;
    public Transform cameraTransform;
    public float suavizacaoRotacao = 0.1f;
    private float velocidadeGiroSuave;

    [Header("Suavidade da Animação")]
    public float animDampTime = 0.15f;

    private float velocidadeVertical;
    private Vector3 deslocamentoPlataforma;
    private Vector3 posicaoCheckpoint;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // Inicializa o checkpoint na posição onde o player começa o jogo
        posicaoCheckpoint = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Mover();
    }

    void Mover()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direcao = new Vector3(h, 0, v).normalized;
        bool estaSeMovendo = direcao.magnitude >= 0.1f;

        // 1. APLICAR MOVIMENTO DA PLATAFORMA (Resolve o tremor)
        if (deslocamentoPlataforma.magnitude > 0)
        {
            controller.Move(deslocamentoPlataforma);
            deslocamentoPlataforma = Vector3.zero;
        }

        // 2. ROTAÇÃO
        Vector3 moveDir = Vector3.zero;
        if (estaSeMovendo)
        {
            float anguloAlvo = Mathf.Atan2(direcao.x, direcao.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angulo = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloAlvo, ref velocidadeGiroSuave, suavizacaoRotacao);
            transform.rotation = Quaternion.Euler(0, angulo, 0);
            moveDir = Quaternion.Euler(0, anguloAlvo, 0) * Vector3.forward;
        }

        // 3. PULO E GRAVIDADE
        if (controller.isGrounded)
        {
            if (velocidadeVertical < 0) velocidadeVertical = -10f; // Gruda no chão

            if (Input.GetButtonDown("Jump"))
            {
                DarImpulsoVertical(forcaPulo);
            }
        }
        else
        {
            velocidadeVertical += gravidade * Time.deltaTime;
        }

        // 4. MOVIMENTO DO TECLADO
        float velocidadeAlvo = Input.GetKey(KeyCode.LeftShift) ? velocidadeCorrer : velocidadeAndar;
        if (!estaSeMovendo) velocidadeAlvo = 0;

        Vector3 movFinalPlayer = (moveDir * velocidadeAlvo) + (Vector3.up * velocidadeVertical);
        controller.Move(movFinalPlayer * Time.deltaTime);

        // 5. ATUALIZAR ANIMATOR
        if (anim != null)
        {
            float velH = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
            anim.SetFloat("Speed", velH, animDampTime, Time.deltaTime);
            anim.SetBool("isMoving", estaSeMovendo);
            anim.SetBool("isGrounded", controller.isGrounded);

            // Danças (Só se estiver parado e no chão)
            if (!estaSeMovendo && controller.isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) anim.SetTrigger("Capoeira");
                if (Input.GetKeyDown(KeyCode.Alpha2)) anim.SetTrigger("Danca");
            }
        }
    }

    // --- MÉTODOS DE INTERAÇÃO (Essenciais para os erros CS1061 sumirem) ---

    public void DefinirCheckpoint(Vector3 novaPos)
    {
        posicaoCheckpoint = novaPos;
    }

    public void Morrer()
    {
        Teletransportar(posicaoCheckpoint);
    }

    public void Teletransportar(Vector3 destino)
    {
        controller.enabled = false;
        transform.position = destino;
        velocidadeVertical = 0;
        controller.enabled = true;
    }

    public void AdicionarMovimentoExterno(Vector3 mov)
    {
        deslocamentoPlataforma += mov;
    }

    public void DarImpulsoVertical(float forca)
    {
        velocidadeVertical = forca;
        if (anim != null) anim.SetTrigger("Jump");
    }
}