using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CloseMenuScene());
    }
    IEnumerator CloseMenuScene()
    {
        yield return new WaitForSeconds(1.9f);
    }
}
