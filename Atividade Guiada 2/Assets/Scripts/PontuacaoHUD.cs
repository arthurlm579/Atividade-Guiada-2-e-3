using UnityEngine;
using TMPro;

public class PontuacaoHUD : MonoBehaviour
{
    public static int pontos = 0;
    private static TextMeshProUGUI textoComponente;

    void Awake()
    {
        textoComponente = GetComponent<TextMeshProUGUI>();
        pontos = 0; // Reseta ao iniciar o jogo
        AtualizarTexto();
    }

    public static void AdicionarPontos(int v)
    {
        pontos += v;
        AtualizarTexto();
    }

    static void AtualizarTexto()
    {
        if (textoComponente != null)
        {
            textoComponente.text = $"Pontos: {pontos}";
        }
    }
}