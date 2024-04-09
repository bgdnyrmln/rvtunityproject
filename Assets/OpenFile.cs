using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class OpenFile : MonoBehaviour
{
    public GameObject placeHolder;



    public void Openf()
    {
        string path = EditorUtility.OpenFilePanel("Overwrite with txt", "", "txt");
        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            var fileContentString = System.Text.Encoding.UTF8.GetString(fileContent);
            placeHolder.GetComponent<InputField>().text = fileContentString;
            Debug.Log(fileContentString);
        }
    }
}
