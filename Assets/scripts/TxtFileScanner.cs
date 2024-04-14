using UnityEngine;
using System.IO;

public class TxtFileScanner : MonoBehaviour
{
    public static string username = "UnknownUser";
    public GameObject fileObjectPrefab;
    public Transform spawnPoint;

    void Start()
    {
        string folderPath = "Assets/users/" + username;
        ScanTxtFiles(folderPath);
    }

    void ScanTxtFiles(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist: " + path);
            return;
        }

        string[] files = Directory.GetFiles(path, "*.txt");
        foreach (string file in files)
        {
            GameObject fileObject = Instantiate(fileObjectPrefab, spawnPoint.position, Quaternion.identity);
            fileObject.transform.SetParent(spawnPoint);
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileObject.GetComponentInChildren<UnityEngine.UI.Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = fileName;
            fileObject.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => OpenFile(file));
        }
    }

    void OpenFile(string filePath)
    {
        System.Diagnostics.Process.Start(filePath);
    }
}
