using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveScreenshot : MonoBehaviour
{
    private string screenshotDirectory;
    // Start is called before the first frame update
    void Start()
    {
        screenshotDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Downloads";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string screenshotName = GenerateFile("Screenshot_" + PlanetHandle.planetToSearch + ".png");
        string fulPath = Path.Combine(screenshotDirectory, screenshotName);

        ScreenCapture.CaptureScreenshot(fulPath);

        Debug.Log("Screenshot guardado!");
    }

    string GenerateFile(string baseFilename)
    {
        string fullPath = Path.Combine(screenshotDirectory, baseFilename);
        string fileWithoutExtension = Path.GetFileNameWithoutExtension(baseFilename);
        string extension = Path.GetExtension(baseFilename);
        int count = 1;

        while (File.Exists(fullPath))
        {
            string tempFilename = $"{fileWithoutExtension} ({count}){extension}";
            fullPath = Path.Combine(screenshotDirectory, tempFilename);
            count++;
        }

        return Path.GetFileName(fullPath);
    }

}
