using UnityEngine;

public class CullObjects : MonoBehaviour
{
    public Transform player; // La cámara o el objeto del jugador
    public float cullDistance = 70f; // Distancia máxima para renderizar objetos
    private Renderer[] renderers; // Array para almacenar los renderers de los objetos

    void Start()
    {
        // Obtener todos los renderers en los hijos de este objeto
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        // Verificar la distancia a la cámara
        float distanceToCamera = Vector3.Distance(transform.position, player.position);

        // Activar o desactivar el renderer dependiendo de la distancia
        foreach (var renderer in renderers)
        {
            renderer.enabled = distanceToCamera <= cullDistance;
        }
    }
}
