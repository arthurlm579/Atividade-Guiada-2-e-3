using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;

    [Header("Configurações de Movimento")]
    public float velocidadeAndar = 6f;
    public float velocidadeCorrer = 10f;
    public float forcaPulo = 11f;
    public float gravidade = -35f;
    public Transform cameraTransform;
    public float suavizacaoRotacao = 0.1f;
    private float velocidadeGiroSuave;

    [Header("Stamina (Sprint)")]
    public float staminaMax = 5f;
    public float staminaAtual;
    public float custoSprintPorSegundo = 1.5f;
    public float recuperacaoStamina = 1f;

    [Header("Double Jump")]
    public int maxSaltos = 2;
    private int saltosRestantes;

    [Header("Wall Slide & Jump")]
    public float velocidadeDeslizamento = 2f;
    public float distanciaParede = 0.7f;
    public LayerMask wallLayer;
    public Vector2 forcaWallJump = new Vector2(8f, 12f);
    private bool encostandoNaParede;

    private float velocidadeVertical;
    private Vector3 deslocamentoPlataforma;
    private Vector3 posicaoCheckpoint;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        staminaAtual = staminaMax;
        saltosRestantes = maxSaltos;
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        posicaoCheckpoint = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. PLATAFORMA MÓVEL
        if (deslocamentoPlataforma.magnitude > 0)
        {
            controller.Move(deslocamentoPlataforma);
            deslocamentoPlataforma = Vector3.zero;
        }

        // 2. INPUT E STAMINA
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direcaoInput = new Vector3(h, 0, v).normalized;

        bool segurandoSprint = Input.GetKey(KeyCode.LeftShift);
        bool podeCorrer = staminaAtual > 0.5f; // Margem de segurança
        bool correndo = segurandoSprint && podeCorrer && controller.isGrounded && direcaoInput.magnitude > 0.1f;

        if (correndo)
            staminaAtual -= custoSprintPorSegundo * Time.deltaTime;
        else
            staminaAtual += recuperacaoStamina * Time.deltaTime;

        staminaAtual = Mathf.Clamp(staminaAtual, 0, staminaMax);

        // 3. DETECÇÃO DE PAREDE (Wall Slide)
        encostandoNaParede = Physics.Raycast(transform.position + Vector3.up, transform.forward, distanciaParede, wallLayer);

        // 4. MOVIMENTO E ROTAÇÃO
        Vector3 moverPara = Vector3.zero;
        if (direcaoInput.magnitude >= 0.1f)
        {
            float anguloAlvo = Mathf.Atan2(direcaoInput.x, direcaoInput.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angulo = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloAlvo, ref velocidadeGiroSuave, suavizacaoRotacao);
            transform.rotation = Quaternion.Euler(0, angulo, 0);
            moverPara = Quaternion.Euler(0, anguloAlvo, 0) * Vector3.forward;
        }

        // 5. PULO / DOUBLE JUMP / WALL JUMP
        if (controller.isGrounded)
        {
            saltosRestantes = maxSaltos;
            if (velocidadeVertical < 0) velocidadeVertical = -2f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (encostandoNaParede && !controller.isGrounded)
            {
                // Wall Jump
                velocidadeVertical = forcaWallJump.y;
                // Impulso para trás da parede
                controller.Move(-transform.forward * forcaWallJump.x * Time.deltaTime);
                if (anim) anim.SetTrigger("Jump");
            }
            else if (saltosRestantes > 0)
            {
                // Pulo Normal e Double Jump
                velocidadeVertical = forcaPulo;
                saltosRestantes--;
                if (anim) { anim.ResetTrigger("Jump"); anim.SetTrigger("Jump"); }
            }
        }

        // 6. GRAVIDADE E WALL SLIDE
        if (encostandoNaParede && !controller.isGrounded && velocidadeVertical < 0)
        {
            velocidadeVertical = -velocidadeDeslizamento; // Desliza devagar
        }
        else
        {
            velocidadeVertical += gravidade * Time.deltaTime;
        }

        float velFinal = correndo ? velocidadeCorrer : velocidadeAndar;
        if (direcaoInput.magnitude < 0.1f) velFinal = 0;

        controller.Move((moverPara * velFinal + Vector3.up * velocidadeVertical) * Time.deltaTime);

        AtualizarAnimacoes(direcaoInput.magnitude >= 0.1f, correndo);
    }

    void AtualizarAnimacoes(bool movendo, bool correndo)
    {
        if (anim == null) return;
        anim.SetFloat("Speed", new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude, 0.1f, Time.deltaTime);
        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetBool("isFalling", !controller.isGrounded && velocidadeVertical < -5f);
        anim.SetBool("WallSlide", encostandoNaParede && !controller.isGrounded && velocidadeVertical < 0);

        if (!movendo && controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) anim.SetTrigger("Capoeira");
            if (Input.GetKeyDown(KeyCode.Alpha2)) anim.SetTrigger("Danca");
        }
    }

    public void Teletransportar(Vector3 destino)
    {
        controller.enabled = false;
        transform.position = destino + Vector3.up * 1f;
        velocidadeVertical = 0;
        staminaAtual = staminaMax;
        controller.enabled = true;
    }

    public void DefinirCheckpoint(Vector3 pos) => posicaoCheckpoint = pos;
    public void Morrer() => Teletransportar(posicaoCheckpoint);
    public void AdicionarMovimentoExterno(Vector3 mov) => deslocamentoPlataforma += mov;
}