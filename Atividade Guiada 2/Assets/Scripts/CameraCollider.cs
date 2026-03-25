using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Configurações de Distância")]
    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float smooth = 10.0f;
    float distance;

    [Header("Configurações de Colisão")]
    public LayerMask collisionLayers; // Marque as camadas Walls, Ground, etc.
    Vector3 dollyDir;

    void Awake()
    {
        // Guarda a direção original da câmera em relação ao "pivot" (pai)
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    void Update()
    {
        // Se a câmera não tiver um pai (Pivot), o script não roda para evitar erros
        if (transform.parent == null) return;

        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        // Faz o raio do centro do Player (pai da câmera) até onde a câmera quer estar
        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit, collisionLayers))
        {
            // Se bater em algo, aproxima a câmera (0.8f é uma margem de segurança)
            distance = Mathf.Clamp((hit.distance * 0.85f), minDistance, maxDistance);
        }
        else
        {
            // Se o caminho estiver livre, volta para a distância máxima
            distance = maxDistance;
        }

        // Move a câmera suavemente para a nova posição
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}