using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;

    [Header("Movimentação")]
    public float velocidadeAndar = 6f;
    public float velocidadeCorrer = 10f;
    public float suavizacaoRotacao = 0.12f;
    private float vGiroSuave;
    public Transform cameraTransform;

    [Header("Estamina")]
    public float staminaMax = 100f;
    public float staminaAtual;
    public float custoCorrida = 20f;
    public float custoWallSlide = 15f;
    public float custoPuloNormal = 10f; // Novo: Custo para cada pulo
    public float custoWallJump = 15f;   // Custo para o pulo na parede
    public float recuperacaoStamina = 15f;
    public Image barraEstamina;

    [Header("Física e Pulo")]
    public float forcaPulo = 11f;
    public float gravidade = -35f;
    private float vVertical;

    [Header("Pulo Duplo")]
    public int maxSaltos = 2;
    private int saltosRestantes;

    [Header("Wall Actions")]
    public LayerMask wallLayer;
    public float distanciaParede = 0.8f;
    public float velDeslizamento = 2f;
    public Vector2 forcaWallJump = new Vector2(12f, 16f);
    private bool estaNaParede;
    private float tempoEsperaGravidade;

    private Vector3 movPlataforma;
    private Vector3 posCheckpoint;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        staminaAtual = staminaMax;
        saltosRestantes = maxSaltos;
        posCheckpoint = transform.position;
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (movPlataforma.magnitude > 0) { controller.Move(movPlataforma); movPlataforma = Vector3.zero; }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dirInput = new Vector3(h, 0, v).normalized;

        estaNaParede = Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, distanciaParede, wallLayer);

        // --- LÓGICA DE ESTAMINA ---
        bool tentandoCorrer = Input.GetKey(KeyCode.LeftShift) && controller.isGrounded && dirInput.magnitude > 0.1f;
        bool tentandoSlide = estaNaParede && !controller.isGrounded && vVertical < 0;

        if (tentandoCorrer && staminaAtual > 0) staminaAtual -= custoCorrida * Time.deltaTime;
        else if (tentandoSlide && staminaAtual > 0) staminaAtual -= custoWallSlide * Time.deltaTime;
        else staminaAtual += recuperacaoStamina * Time.deltaTime;

        staminaAtual = Mathf.Clamp(staminaAtual, 0, staminaMax);
        if (barraEstamina != null) barraEstamina.fillAmount = staminaAtual / staminaMax;

        float velFinal = (tentandoCorrer && staminaAtual > 0) ? velocidadeCorrer : velocidadeAndar;

        // --- ROTAÇÃO ---
        Vector3 moverPara = Vector3.zero;
        if (dirInput.magnitude >= 0.1f)
        {
            float anguloAlvo = Mathf.Atan2(dirInput.x, dirInput.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angulo = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloAlvo, ref vGiroSuave, suavizacaoRotacao);
            transform.rotation = Quaternion.Euler(0, angulo, 0);
            moverPara = Quaternion.Euler(0, anguloAlvo, 0) * Vector3.forward;
        }

        // --- PULO E WALL JUMP COM GASTO ---
        if (controller.isGrounded)
        {
            saltosRestantes = maxSaltos;
            if (vVertical < 0) vVertical = -2f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            // Tenta Wall Jump
            if (estaNaParede && !controller.isGrounded && staminaAtual >= custoWallJump)
            {
                vVertical = forcaWallJump.y;
                tempoEsperaGravidade = 0.15f;

                Vector3 impulsoFora = -transform.forward * forcaWallJump.x;
                controller.Move(impulsoFora * Time.deltaTime);

                anim.SetTrigger("Jump");
                staminaAtual -= custoWallJump; // Gasta estamina
                saltosRestantes = maxSaltos - 1;
            }
            // Tenta Pulo Normal ou Duplo
            else if (saltosRestantes > 0 && staminaAtual >= custoPuloNormal)
            {
                vVertical = forcaPulo;
                if (saltosRestantes == maxSaltos) anim.SetTrigger("Jump");
                else anim.SetTrigger("DoubleJumpTrigger");

                staminaAtual -= custoPuloNormal; // Gasta estamina
                saltosRestantes--;
            }
        }

        // --- GRAVIDADE ---
        if (tempoEsperaGravidade > 0) tempoEsperaGravidade -= Time.deltaTime;
        else if (tentandoSlide && staminaAtual > 0) vVertical = -velDeslizamento;
        else vVertical += gravidade * Time.deltaTime;

        controller.Move((moverPara * velFinal + Vector3.up * vVertical) * Time.deltaTime);

        // --- ANIMATOR ---
        if (anim != null)
        {
            anim.SetBool("isGrounded", controller.isGrounded);
            anim.SetFloat("Speed", dirInput.magnitude * velFinal, 0.15f, Time.deltaTime);
            anim.SetBool("isFalling", !controller.isGrounded && vVertical < -2f && !estaNaParede);
            anim.SetBool("WallSlide", tentandoSlide && staminaAtual > 0);
        }
    }

    public void AdicionarMovimentoExterno(Vector3 mov) => movPlataforma += mov;
    public void Morrer() => Teletransportar(posCheckpoint);
    public void DefinirCheckpoint(Vector3 pos) => posCheckpoint = pos;
    public void Teletransportar(Vector3 dest) { controller.enabled = false; transform.position = dest; vVertical = 0; controller.enabled = true; }
}