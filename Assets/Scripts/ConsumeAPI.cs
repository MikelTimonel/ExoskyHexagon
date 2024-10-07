using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ApiConsumer : MonoBehaviour
{
    // URL de la API
    private string url = "https://nasa-entorno.onrender.com/planets";

    // Llamamos a la funci�n Start al iniciar el script
    void Start()
    {
        StartCoroutine(GetPlanets());
    }

    // Funci�n que realiza la solicitud a la API
    IEnumerator GetPlanets()
    {
        // Hacemos una solicitud GET a la URL
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Enviamos la solicitud y esperamos la respuesta
        yield return request.SendWebRequest();

        // Verificamos si hay alg�n error en la solicitud
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al consumir la API: " + request.error);
        }
        else
        {
            // Si la solicitud es exitosa, mostramos el resultado en consola
            Debug.Log("Respuesta de la API: " + request.downloadHandler.text);

            // Opcional: Puedes deserializar el JSON si es necesario
            // Por ejemplo, podr�as usar JsonUtility o Newtonsoft.Json para manejar la deserializaci�n
            // var planetsData = JsonUtility.FromJson<List<Planet>>(request.downloadHandler.text);
            // Debug.Log("Planeta: " + planetsData[0].name);
        }
    }
}
