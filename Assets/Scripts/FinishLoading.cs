using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FinishLoading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeScene());
    }

    // Update is called once per frame
    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(6.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Exosky");

    }
}
