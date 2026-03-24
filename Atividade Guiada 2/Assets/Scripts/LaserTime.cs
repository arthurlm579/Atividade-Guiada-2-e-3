using UnityEngine;

public class LaserTimer : MonoBehaviour
{
    public float tempoLigado = 2f;    // Quanto tempo o laser fica visível
    public float tempoDesligado = 2f; // Quanto tempo ele sumido

    private MeshRenderer visual;
    private Collider colisor;
    private float cronometro;
    private bool estaLigado = true;

    void Start()
    {
        // Pega os componentes do laser
        visual = GetComponent<MeshRenderer>();
        colisor = GetComponent<Collider>();
    }

    void Update()
    {
        cronometro += Time.deltaTime;

        if (estaLigado && cronometro >= tempoLigado)
        {
            AlternarLaser(false);
        }
        else if (!estaLigado && cronometro >= tempoDesligado)
        {
            AlternarLaser(true);
        }
    }

    void AlternarLaser(bool valor)
    {
        estaLigado = valor;
        cronometro = 0;

        // Esconde o visual
        if (visual != null) visual.enabled = valor;

        // Desativa o colisor (para não matar o player quando estiver invisível)
        if (colisor != null) colisor.enabled = valor;
    }
}