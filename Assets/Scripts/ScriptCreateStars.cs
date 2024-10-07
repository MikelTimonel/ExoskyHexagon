using UnityEngine;

public class RandomPrefabSpawner : MonoBehaviour
{
    public GameObject prefab; // Asigna tu prefab en el inspector
    public int numberOfInstances = 400; // N�mero de instancias a crear
    public float radius = 20f; // Radio de creaci�n

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Obt�n la c�mara principal

    }
    void Update()
    {
        // Detecta si se presiona la tecla "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnPrefabs(); // Llama al m�todo para crear las instancias
        }
    }
    void SpawnPrefabs()
    {
        for (int i = 0; i < numberOfInstances; i++)
        {
            // Genera una posici�n aleatoria dentro de un radio alrededor de la c�mara
            Vector3 randomPosition = GenerateRandomPosition();
            // Instancia el prefab en la posici�n aleatoria
            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }

    Vector3 GenerateRandomPosition()
    {
        // Genera una posici�n aleatoria en un c�rculo en el plano XY y en el eje Z
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        float randomHeight = Random.Range(-radius, radius); // Para la altura aleatoria
        Vector3 randomPosition = new Vector3(randomCircle.x, randomHeight, randomCircle.y);

        // Coloca la posici�n en relaci�n con la posici�n de la c�mara
        return mainCamera.transform.position + randomPosition;
    }
}
