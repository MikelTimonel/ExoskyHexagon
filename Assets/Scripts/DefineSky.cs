using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DefineSky : MonoBehaviour
{
    public TextMeshProUGUI textField; // Cambiar a TMP_InputField

    // Start is called before the first frame update
    void Start()
    {
        Material skyboxMaterial = null;
        if (PlanetHandle.planetToSearch != "")
        {
            skyboxMaterial = Resources.Load<Material>(PlanetHandle.planetToSearch + "/ExportedSkybox");
            textField.text = PlanetHandle.planetToSearch;
        }
        else
        {
            List<string> planets = new List<string> { "TOI-199 c", "Kepler-327 d", "HAT-P-7 b", "HD 17156 b", "HAT-P-18 b", "HD 104985 b", "HD 132563 b", "kap CrB b", "PDS 70 b" };

            int indexA = Random.Range(0, planets.Count );
            string planet = planets[indexA];
            skyboxMaterial = Resources.Load<Material>(planet + "/ExportedSkybox");
            PlanetHandle.planetToSearch = planet;
            textField.text = PlanetHandle.planetToSearch;

        }
        if (skyboxMaterial != null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera.GetComponent<Skybox>() == null)
            {
                mainCamera.gameObject.AddComponent<Skybox>();
            }
            mainCamera.GetComponent<Skybox>().material = skyboxMaterial;
        }

    }
}
