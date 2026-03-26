using UnityEngine;
using TMPro; // Necessário para usar TextMeshPro
using System.Collections;

public class TutorialTexto : MonoBehaviour
{
    public TextMeshProUGUI campoDeTexto; // Arraste o texto do Canvas aqui
    public string mensagem = "Nova Área Descoberta!";
    public float tempoExibicao = 3f;
    private bool jaAtivou = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o Player e se ainda não ativou (para não repetir toda hora)
        if (other.CompareTag("Player") && !jaAtivou)
        {
            StartCoroutine(MostrarTexto());
            jaAtivou = true;
        }
    }

    IEnumerator MostrarTexto()
    {
        campoDeTexto.text = mensagem;
        campoDeTexto.gameObject.SetActive(true); // Mostra o texto

        yield return new WaitForSeconds(tempoExibicao); // Espera o tempo definido

        campoDeTexto.gameObject.SetActive(false); // Esconde o texto

        // Se quiser que o texto apareça toda vez que entrar, apague a linha "jaAtivou = true" acima.
    }
}