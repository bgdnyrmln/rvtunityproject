using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TxtFileScanner : MonoBehaviour
{
    public static string username = "UnknownUser";
    public GameObject fileObjectPrefab;
    public Transform spawnPoint;
    public Button backToLogingScreen;
    public Button createFile;
    public GameObject createNewButton; // тут € костыль сделал два обьекта чтобы через баттон сделать листенер и через обьект сделать активным или не активным хз как иначе

    void Start()
    {
        backToLogingScreen.onClick.AddListener(backToLogin);
        createFile.onClick.AddListener(createFileAction);
        string folderPath = "Assets/users/" + username;
        ScanTxtFiles(folderPath);
    }

    void ScanTxtFiles(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist: " + path);
            createNewButton.SetActive(true);
            return;
        }

        string[] files = Directory.GetFiles(path, "*.txt");
        if (files.Length == 0)
        {

            createNewButton.SetActive(true);
            return;
        }

        foreach (string file in files)
        {
            GameObject fileObject = Instantiate(fileObjectPrefab, spawnPoint.position, Quaternion.identity);
            fileObject.transform.SetParent(spawnPoint);
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileObject.GetComponentInChildren<UnityEngine.UI.Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = fileName;

            string fileContent = System.IO.File.ReadAllText(file);

            UnityEngine.UI.Button button = fileObject.GetComponentInChildren<UnityEngine.UI.Button>();
            button.onClick.AddListener(() => LoadNotepadScene(fileContent));
        }
    }

    void OpenFile(string filePath)
    {
        System.Diagnostics.Process.Start(filePath);
    }

    void backToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    void createFileAction()
    {
        SceneManager.LoadScene("Notepad");
    }
    void LoadNotepadScene(string content)
    {
        PlayerPrefs.SetString("FileContent", content);
        SceneManager.LoadScene("Notepad");
    }
}