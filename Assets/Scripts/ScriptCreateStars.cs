using UnityEngine;

public class RandomPrefabSpawner : MonoBehaviour
{
    public GameObject prefab; // Asigna tu prefab en el inspector
    public int numberOfInstances = 400; // Número de instancias a crear
    public float radius = 20f; // Radio de creación

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Obtén la cámara principal

    }
    void Update()
    {
        // Detecta si se presiona la tecla "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnPrefabs(); // Llama al método para crear las instancias
        }
    }
    void SpawnPrefabs()
    {
        for (int i = 0; i < numberOfInstances; i++)
        {
            // Genera una posición aleatoria dentro de un radio alrededor de la cámara
            Vector3 randomPosition = GenerateRandomPosition();
            // Instancia el prefab en la posición aleatoria
            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }

    Vector3 GenerateRandomPosition()
    {
        // Genera una posición aleatoria en un círculo en el plano XY y en el eje Z
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        float randomHeight = Random.Range(-radius, radius); // Para la altura aleatoria
        Vector3 randomPosition = new Vector3(randomCircle.x, randomHeight, randomCircle.y);

        // Coloca la posición en relación con la posición de la cámara
        return mainCamera.transform.position + randomPosition;
    }
}
