using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class SavePlane : MonoBehaviour
{
    public TMP_InputField inputField; // Cambiar a TMP_InputField
    public GameObject animator;

    public void StoreInput()
    {
        // Almacena el texto del InputField en la clase estática
        PlanetHandle.planetToSearch = inputField.text;

        StartCoroutine(CloseMenuScene());
    }
    IEnumerator CloseMenuScene()
    {
        animator.SetActive(true);
        yield return new WaitForSeconds(1.9f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScreen");
    }

    public void RandomScene()
    {
        PlanetHandle.planetToSearch = "";
        StartCoroutine(CloseMenuScene());
    }
}