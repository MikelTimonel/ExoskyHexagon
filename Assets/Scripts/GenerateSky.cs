using UnityEditor;
using System.IO;
using UnityEngine;
using static GenerateSky;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using System.Numerics;
using System.Collections.Generic;

public class GenerateSky : MonoBehaviour
{
    private string url = "http://127.0.0.1:8000/stars/";
    private string faceTexture;
    public int textureWidth = 720; // Tamaño de la textura
    public int textureHeight = 720;
    public int minPoints = 700;     // Mínimo número de puntos en cada textura
    public int maxPoints = 1600;     // Máximo número de puntos en cada textura
    public int minSize = 1;         // Tamaño mínimo del punto
    public int maxSize = 4;        // Tamaño máximo del punto
    List<UnityEngine.Vector2> pointsFront= new List<UnityEngine.Vector2>();
    List<UnityEngine.Vector2> pointsBack = new List<UnityEngine.Vector2>();
    List<UnityEngine.Vector2> pointsLeft = new List<UnityEngine.Vector2>();
    List<UnityEngine.Vector2> pointsRight = new List<UnityEngine.Vector2>();
    List<UnityEngine.Vector2> pointsUp = new List<UnityEngine.Vector2>();
    List<UnityEngine.Vector2> pointsDown= new List<UnityEngine.Vector2>();
    [System.Serializable]
    public class PlanetData
    {
        public int dec;
        public string pl_name;
        public int pl_rade;
        public int pl_radj;
        public int ra;
        public int st_rad;
        public int sy_dist;
        public int sy_vmag;
    }
    [System.Serializable]
    public class PlanetsList
    {
        public PlanetData[] planets;
    }
    [System.Serializable]
    public class StarData
    {
        public int ra;
        public int dec;
        public int distance;
        public int phot_g;
        public int phot_bp;
        public int phot_rp;
    }
    [System.Serializable]
    public class StarsList
    {
        public StarData[] stars;
    }

    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "Planets.txt");
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            PlanetsList planetList = JsonUtility.FromJson<PlanetsList>(jsonContent);
            StartCoroutine(ProcessPlanets(planetList));
            
        }
    }

    IEnumerator ProcessPlanets(PlanetsList planetList)
    {
        int counter = 0;
        foreach (PlanetData planet in planetList.planets)
        {
            if (counter < 22)
            {
                // Llama a la corutina GetStars y espera a que termine
                yield return GetStars(planet.pl_name, planet.ra, planet.dec, planet.sy_dist);
            } else
            {
                break;
            }
            counter++;
            pointsFront.Clear();
            pointsLeft.Clear();
            pointsRight.Clear();
            pointsBack.Clear();
            pointsUp.Clear();
            pointsDown.Clear();
        }
    }

    private void SaveTextureAsPNG(Texture2D texture, string filePath)
    {
        byte[] pngData = texture.EncodeToPNG(); // Convertir la textura a PNG.
        if (pngData != null)
        {
            File.WriteAllBytes(filePath, pngData);
        }
    }
    // Función para crear una textura negra con puntos blancos de tamaños y ubicaciones aleatorias
    Texture2D CreateTexture(int width, int height, List<UnityEngine.Vector2> pointCoordinates)
    {
        Texture2D texture = new Texture2D(width, height);

        // Rellenar toda la textura con color negro
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }

        texture.SetPixels(pixels); // Aplicar color de fondo negro

        // Generar puntos blancos de tamaños y ubicaciones aleatorias
        int numberOfPoints = Random.Range(minPoints, maxPoints); // Número aleatorio de puntos
        foreach (UnityEngine.Vector2 point in pointCoordinates)
        {
            int pointSize =  Random.Range(minSize, maxSize);       // Tamaño aleatorio de cada punto

            int x = (int)point.x;
            int y = (int)point.y;

            // Dibujar el punto blanco en la textura según el tamaño especificado
            for (int dx = 0; dx < pointSize; dx++)
            {
                for (int dy = 0; dy < pointSize; dy++)
                {
                    if (x + dx < width && y + dy < height) // Verifica que el punto esté dentro de los límites de la textura
                    {
                        texture.SetPixel(x + dx, y + dy, Color.white); // Dibujar punto blanco
                    }
                }
            }
        }

        texture.Apply(); // Aplicar los cambios a la textura
        return texture;
    }
    IEnumerator GetStars(string planetName, int ra, int dec, int dist)
    {
        UnityWebRequest request = UnityWebRequest.Get(url + planetName);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error en la solicitud: " + request.error);
        }
        else
        {
            string path = Path.Combine(Application.dataPath, planetName);

            // Verificar si el directorio ya existe para no crear duplicados
            if (!Directory.Exists(path))
            {
                // Crear el directorio
                Directory.CreateDirectory(path);
            }
            string jsonResponse = request.downloadHandler.text;
            StarsList stars = JsonUtility.FromJson<StarsList>(jsonResponse);
            
            foreach (StarData star in stars.stars)
            {
                convertToCartesian(star.ra, star.dec, star.distance, ra,dec, dist);
            }
            Material skyboxMaterial = new Material(Shader.Find("Custom/NewSurfaceShader"));

            // Generar las texturas negras con puntos blancos de tamaños aleatorios
            Texture2D frontTexture = CreateTexture(textureWidth, textureHeight, pointsFront);
            Texture2D backTexture = CreateTexture(textureWidth, textureHeight, pointsBack);
            Texture2D leftTexture = CreateTexture(textureWidth, textureHeight, pointsLeft);
            Texture2D rightTexture = CreateTexture(textureWidth, textureHeight, pointsRight);
            Texture2D upTexture = CreateTexture(textureWidth, textureHeight, pointsUp);
            Texture2D downTexture = CreateTexture(textureWidth, textureHeight, pointsDown);
            skyboxMaterial.SetTexture("_FrontTex", frontTexture);
            skyboxMaterial.SetTexture("_BackTex", backTexture);
            skyboxMaterial.SetTexture("_LeftTex", leftTexture);
            skyboxMaterial.SetTexture("_RightTex", rightTexture);
            skyboxMaterial.SetTexture("_UpTex", upTexture);
            skyboxMaterial.SetTexture("_DownTex", downTexture);

            // Aplicar el material del skybox
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment(); // Actualizar la iluminación del entorno
            #if UNITY_EDITOR
                AssetDatabase.CreateAsset(skyboxMaterial, "Assets/" + planetName + "/ExportedSkybox.mat");
            #endif
            SaveTextureAsPNG(frontTexture, "Assets/" + planetName + "/frontTexture.png");
            SaveTextureAsPNG(backTexture, "Assets/" + planetName + "/backTexture.png");
            SaveTextureAsPNG(leftTexture, "Assets/" + planetName + "/leftTexture.png");
            SaveTextureAsPNG(rightTexture, "Assets/" + planetName + "/rightTexture.png");
            SaveTextureAsPNG(upTexture, "Assets/" + planetName + "/upTexture.png");
            SaveTextureAsPNG(downTexture, "Assets/" + planetName + "/downTexture.png");
            #if UNITY_EDITOR
                AssetDatabase.SaveAssets();
            #endif
        }
    }
    void convertToCartesian(int ra, int dec, int distance, int ra_p, int dec_p, int dist_p)
    {
        float ra_rad = ra * Mathf.Deg2Rad;
        float dec_rad = dec * Mathf.Deg2Rad;
        float ra_p_rad = ra_p * Mathf.Deg2Rad;
        float dec_p_rad = dec_p * Mathf.Deg2Rad;
        float x = (distance * Mathf.Cos(dec_rad) * Mathf.Cos(ra_rad)) - (dist_p * Mathf.Cos(dec_p_rad) * Mathf.Cos(ra_p_rad));
        float y = (distance * Mathf.Cos(dec_rad) * Mathf.Sin(ra_rad)) - (dist_p * Mathf.Cos(dec_p_rad) * Mathf.Sin(ra_p_rad));
        float z = (distance * Mathf.Sin(dec_rad)) - (dist_p * Mathf.Sin(dec_p_rad));
        /*if (Mathf.Abs(z) > Mathf.Abs(x) && Mathf.Abs(z) > Mathf.Abs(y))
        {
            faceTexture = (z > 0) ? "Front" : "Back";
        }
        else if (Mathf.Abs(x) > Mathf.Abs(y) && Mathf.Abs(x) > Mathf.Abs(z))
        {
            faceTexture = x > 0 ? "Right" : "Left";
        }
        else
        {
            faceTexture = (y > 0) ? "Top" : "Bottom";
        }*/
        UnityEngine.Vector2 positionTexture = ProyectarEnSkybox(x, y, z, 1980);
        if (faceTexture == "Right")
        {
            pointsRight.Add(positionTexture);
        }
        else if (faceTexture == "Left")
        {
            pointsLeft.Add(positionTexture);
        }
        else if (faceTexture == "Up")
        {
            pointsUp.Add(positionTexture);
        }
        else if (faceTexture == "Down")
        {
            pointsDown.Add(positionTexture);
        }
        else if (faceTexture == "Front")
        {
            pointsFront.Add(positionTexture);
        }
        else if (faceTexture == "Back")
        {
            pointsBack.Add(positionTexture);
        }
    }
    // Función para obtener la posición en la textura dado x, y, z
    UnityEngine.Vector2 ProyectarEnSkybox(float x, float y, float z, int resolution)
    {
        float u, v;
        // Decidir en qué cara del cubo se proyecta
        if (Mathf.Abs(x) > Mathf.Abs(y) && Mathf.Abs(x) > Mathf.Abs(z))
        {
            // Cara derecha o izquierda
            if (x > 0)
            {
                // Cara derecha
                u = -z / x;
                v = y / x;
                faceTexture = "Right";
                //pointsRight.Add(new UnityEngine.Vector2(u, v));
            }
            else
            {
                // Cara izquierda
                u = z / x;
                v = y / x;
                faceTexture = "Left";
            }
        }
        else if (Mathf.Abs(y) > Mathf.Abs(x) && Mathf.Abs(y) > Mathf.Abs(z))
        {
            // Cara superior o inferior
            if (y > 0)
            {
                // Cara superior
                u = x / y;
                v = z / y;
                faceTexture = "Up";
            }
            else
            {
                // Cara inferior
                u = x / y;
                v = -z / y;
                faceTexture = "Down";
            }
        }
        else
        {
            // Cara frontal o trasera
            if (z > 0)
            {
                // Cara frontal
                u = x / z;
                v = y / z;
                faceTexture = "Front";
            }
            else
            {
                // Cara trasera
                u = -x / z;
                v = y / z;
                faceTexture = "Back";
            }
        }

        // Normalizar al rango [0, 1]
        u = (u + 1) / 2;
        v = (v + 1) / 2;

        // Convertir a píxeles
        u *= resolution;
        v *= resolution;

        return new UnityEngine.Vector2(u, v);
    }

}
