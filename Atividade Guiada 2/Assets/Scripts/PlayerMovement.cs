using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;

    [Header("Referências")]
    public Transform cameraTransform;

    [Header("Velocidades")]
    public float velocidadeAndar = 6f;
    public float velocidadeCorrer = 12f;
    public float forcaPulo = 8f;
    public float suavizacaoRotacao = 0.1f; // Tempo para o boneco girar
    private float velocidadeGiroSuave;

    [Header("Física")]
    public float gravidade = -25f;
    private float velocidadeVertical;
    private Vector3 direcaoMovimento;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Busca o animator no modelo 3D que está dentro da cápsula
        anim = GetComponentInChildren<Animator>();

        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // Esconde o cursor do mouse para facilitar o teste
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CalcularMovimentoERotacao();
        AplicarGravidadeEPulo();
        AtualizarAnimacoes();
    }

    void CalcularMovimentoERotacao()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direcaoInput = new Vector3(x, 0, z).normalized;

        if (direcaoInput.magnitude >= 0.1f)
        {
            // CALCULA A ROTAÇÃO: Faz o boneco olhar para a direção da câmera + input
            float anguloAlvo = Mathf.Atan2(direcaoInput.x, direcaoInput.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float anguloSuave = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloAlvo, ref velocidadeGiroSuave, suavizacaoRotacao);

            // GIRA A CÁPSULA (O robô gira junto porque é filho)
            transform.rotation = Quaternion.Euler(0, anguloSuave, 0);

            // Define a direção para onde o controlador vai se mover
            direcaoMovimento = Quaternion.Euler(0, anguloAlvo, 0) * Vector3.forward;
        }
        else
        {
            direcaoMovimento = Vector3.zero;
        }

        // Define se está correndo ou andando
        float velocidadeFinal = Input.GetKey(KeyCode.LeftShift) ? velocidadeCorrer : velocidadeAndar;

        // Move o Character Controller
        Vector3 movimentoFinal = direcaoMovimento * velocidadeFinal;
        movimentoFinal.y = velocidadeVertical;
        controller.Move(movimentoFinal * Time.deltaTime);
    }

    void AplicarGravidadeEPulo()
    {
        if (controller.isGrounded)
        {
            if (velocidadeVertical < 0) velocidadeVertical = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                velocidadeVertical = forcaPulo;
                if (anim != null) anim.SetTrigger("Jump");
            }
        }
        else
        {
            velocidadeVertical += gravidade * Time.deltaTime;
        }
    }

    void AtualizarAnimacoes()
    {
        if (anim != null)
        {
            // Pega a velocidade horizontal (sem o Y do pulo)
            float velHorizontal = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

            // Envia para o parâmetro "Speed" do Animator
            anim.SetFloat("Speed", velHorizontal);
        }
    }
}