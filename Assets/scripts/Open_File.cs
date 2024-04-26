using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SFB;

public class FileOpener : MonoBehaviour
{
    public GameObject placeHolder;

    public void OpenFile()
    {
        // Open a file dialog to select a text file
        string path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "txt", false)[0];

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                // Read the contents of the selected text file
                string fileContent = File.ReadAllText(path);

                // Set the text content to the input field
                placeHolder.GetComponent<InputField>().text = fileContent;

                Debug.Log("File content: " + fileContent);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error reading file: " + ex.Message);
            }
        }
    }
}
