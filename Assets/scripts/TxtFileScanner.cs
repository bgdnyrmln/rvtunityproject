using UnityEngine;
using System.IO;

public class TxtFileScanner : MonoBehaviour
{
    //СЮДА НАДО ДОБАВИТЬ ЭТУ ХУЙНЮ С ЮЗЕРНЕЙМОМ короче понял чтобы он читал юзернейм и заходил в его папку
    public static string username = "UnknownUser";
    //public static string username = PlayerPrefs.GetString("Username", "UnknownUser");
    public string folderPath = "Assets/users/" + username;
    public GameObject fileObjectPrefab;
    public Transform spawnPoint; 

    void Start()
    {
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
