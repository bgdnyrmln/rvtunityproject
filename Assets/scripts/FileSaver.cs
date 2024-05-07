using UnityEngine;
using System;
using System.IO;

public class FileSaver
{
    public static bool SaveText(string text, string fileName, string username)
    {
        try
        {
            // Combine the current directory and the file name to get the full path
            string filePath = Application.dataPath + "/" + "users" + "/" + username + "/" + fileName;
            
            // Write the text to the file
            File.WriteAllText(filePath, text);

            Debug.Log("File saved to: " + filePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving file: " + e.Message);
            return false;
        }
    }
}
