using UnityEngine;

public class PortaInterativa : MonoBehaviour
{
    public Transform dobradica;
    public bool aberta = false;
    public float distanciaInteracao = 3f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, player.position);

        if (Input.GetKeyDown(KeyCode.E) && distancia <= distanciaInteracao)
        {
            aberta = !aberta;
        }

        float angulo = aberta ? 90f : 0f;
        Quaternion targetRotation = Quaternion.Euler(0, angulo, 0);

        // Suaviza a abertura
        dobradica.localRotation = Quaternion.Slerp(dobradica.localRotation, targetRotation, Time.deltaTime * 5f);
    }
}