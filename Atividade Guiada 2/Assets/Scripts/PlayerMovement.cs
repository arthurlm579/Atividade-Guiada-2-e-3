using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // 1. Adicionado para o novo sistema de texto

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
    public float custoPulo = 12f;
    public float custoWallJump = 15f;
    public float custoWallSlide = 15f; // 2. Corrigido: Agora o erro CS0103 some
    public float recuperacaoStamina = 15f;
    public Image barraEstamina;

    [Header("Física")]
    public float forcaPulo = 11f;
    public float gravidade = -35f;
    private float vVertical;
    public int maxSaltos = 2;
    private int saltosRestantes;

    [Header("Wall Actions")]
    public LayerMask wallLayer;
    public float distanciaParede = 0.8f;
    public float velDeslizamento = 2f;
    public Vector2 forcaWallJump = new Vector2(12f, 16f);
    private bool estaNaParede;
    private float travaGravidade;

    [Header("Menus (UI)")]
    public GameObject painelDerrota;
    public GameObject painelVitoria;
    public GameObject menuPausa;
    private bool jogoPausado = false;

    [Header("Sistema de Coleta")]
    public int moedasColetadas = 0;
    public int moedasParaVencer = 5;
    public TextMeshProUGUI textoContador; // 3. Corrigido: Agora aceita arrastar o texto do Canvas

    private Vector3 movPlataforma = Vector3.zero;
    private Vector3 posCheckpoint;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        staminaAtual = staminaMax;
        posCheckpoint = transform.position;

        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        AtualizarTextoMoedas();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (painelDerrota != null && !painelDerrota.activeSelf && !painelVitoria.activeSelf)
                AlternarPausa(!jogoPausado);
        }

        if (jogoPausado) return;

        // Movimento de Plataformas
        if (movPlataforma.magnitude > 0.01f)
        {
            controller.Move(movPlataforma);
            movPlataforma = Vector3.zero;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 inputDir = new Vector2(h, v).normalized;

        estaNaParede = Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, distanciaParede, wallLayer);

        // Lógica de Estamina
        bool tentandoCorrer = Input.GetKey(KeyCode.LeftShift) && controller.isGrounded && inputDir.magnitude > 0.1f;
        bool tentandoSlide = estaNaParede && !controller.isGrounded && vVertical < 0;

        if (tentandoCorrer && staminaAtual > 0) staminaAtual -= custoCorrida * Time.deltaTime;
        else if (tentandoSlide && staminaAtual > 0) staminaAtual -= custoWallSlide * Time.deltaTime;
        else staminaAtual += recuperacaoStamina * Time.deltaTime;

        staminaAtual = Mathf.Clamp(staminaAtual, 0, staminaMax);
        if (barraEstamina != null) barraEstamina.fillAmount = staminaAtual / staminaMax;

        float velFinal = (tentandoCorrer && staminaAtual > 0) ? velocidadeCorrer : velocidadeAndar;

        // Rotação
        Vector3 moverPara = Vector3.zero;
        if (inputDir.magnitude >= 0.1f)
        {
            float anguloAlvo = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angulo = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloAlvo, ref vGiroSuave, suavizacaoRotacao);
            transform.rotation = Quaternion.Euler(0, angulo, 0);
            moverPara = Quaternion.Euler(0, anguloAlvo, 0) * Vector3.forward;
        }

        // Pulos
        if (controller.isGrounded)
        {
            saltosRestantes = maxSaltos;
            if (vVertical < 0) vVertical = -2f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (estaNaParede && !controller.isGrounded && staminaAtual >= custoWallJump)
            {
                vVertical = forcaWallJump.y;
                travaGravidade = 0.15f;
                controller.Move(-transform.forward * forcaWallJump.x * Time.deltaTime);
                if (anim != null) anim.SetTrigger("Jump");
                staminaAtual -= custoWallJump;
                saltosRestantes = maxSaltos - 1;
            }
            else if (saltosRestantes > 0 && staminaAtual >= custoPulo)
            {
                vVertical = forcaPulo;
                if (anim != null)
                {
                    if (saltosRestantes == maxSaltos) anim.SetTrigger("Jump");
                    else anim.SetTrigger("DoubleJumpTrigger");
                }
                staminaAtual -= custoPulo;
                saltosRestantes--;
            }
        }

        // Gravidade
        if (travaGravidade > 0) travaGravidade -= Time.deltaTime;
        else if (tentandoSlide && staminaAtual > 0) vVertical = -velDeslizamento;
        else vVertical += gravidade * Time.deltaTime;

        controller.Move((moverPara * velFinal + Vector3.up * vVertical) * Time.deltaTime);

        // Animações
        if (anim != null)
        {
            anim.SetBool("isGrounded", controller.isGrounded);
            anim.SetFloat("Speed", inputDir.magnitude * velFinal, 0.1f, Time.deltaTime);
            anim.SetBool("WallSlide", tentandoSlide && staminaAtual > 0);
        }
    }

    // --- MÉTODOS DE SUPORTE (Checkpoint e Plataforma) ---
    public void DefinirCheckpoint(Vector3 pos) { posCheckpoint = pos; } // Resolve erro CS1061
    public void AdicionarMovimentoExterno(Vector3 mov) { movPlataforma += mov; } // Resolve erro CS1061

    // --- SISTEMA DE COLETA E VITÓRIA ---
    public void ColetarMoeda()
    {
        moedasColetadas++;
        AtualizarTextoMoedas();
        if (moedasColetadas >= moedasParaVencer) Vencer();
    }

    void AtualizarTextoMoedas()
    {
        if (textoContador != null)
            textoContador.text = "Moedas: " + moedasColetadas + " / " + moedasParaVencer;
    }

    void Vencer()
    {
        if (painelVitoria != null) painelVitoria.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        jogoPausado = true;
    }

    public void Morrer()
    {
        if (painelDerrota != null) painelDerrota.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        jogoPausado = true;
    }

    public void ReententarDoCheckpoint()
    {
        Time.timeScale = 1f;
        jogoPausado = false;
        if (painelDerrota != null) painelDerrota.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Teletransportar(posCheckpoint);
    }

    public void ReiniciarFaseInteira()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AlternarPausa(bool pausar)
    {
        jogoPausado = pausar;
        if (menuPausa != null) menuPausa.SetActive(pausar);
        Time.timeScale = pausar ? 0f : 1f;
        Cursor.lockState = pausar ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void Teletransportar(Vector3 dest)
    {
        controller.enabled = false;
        transform.position = dest;
        vVertical = 0;
        controller.enabled = true;
    }
}