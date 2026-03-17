using UnityEngine;

public class EfeitoMoeda : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    public float velocidadeGiro = 100f;

    [Header("Configurações de Flutuação")]
    public float amplitude = 0.5f; // O quão alto ela vai
    public float frequencia = 2f;  // A velocidade da subida/descida

    private Vector3 posicaoInicial;

    void Start()
    {
        // Salva a posição onde você colocou a moeda no cenário
        posicaoInicial = transform.position;
    }

    void Update()
    {
        // 1. Faz a moeda girar
        transform.Rotate(Vector3.up * velocidadeGiro * Time.deltaTime);

        // 2. Faz a moeda flutuar usando uma onda de Seno (Mathf.Sin)
        float novoY = posicaoInicial.y + Mathf.Sin(Time.time * frequencia) * amplitude;
        transform.position = new Vector3(posicaoInicial.x, novoY, posicaoInicial.z);
    }
}