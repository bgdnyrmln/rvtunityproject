using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TxtFileScanner : MonoBehaviour
{
    public static string username = "UnknownUser";
    public GameObject fileObjectPrefab;
    public Transform spawnPoint;
    public Button backToLogingScreen;
    public Button createFile;
    public Button SortByName;
    public Button SortByDate;
    public GameObject createNewButton;
    public GameObject placeHolder;
    public float buttonWidth = 100f;
    public float buttonHeight = 60f;
    public float verticalGap = 15f;
    public GameObject deleteButtonPrefab; // Add reference to the delete button prefab

    void Start()
    {
        backToLogingScreen.onClick.AddListener(backToLogin);
        createFile.onClick.AddListener(createFileAction);
        SortByName.onClick.AddListener(SortByNameFunction);
        SortByDate.onClick.AddListener(SortByDateFunction);
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

        // Sort files by name
        Array.Sort(files, (a, b) => string.Compare(Path.GetFileNameWithoutExtension(a), Path.GetFileNameWithoutExtension(b)));

        float panelWidth = spawnPoint.GetComponent<RectTransform>().rect.width;
        float panelHeight = spawnPoint.GetComponent<RectTransform>().rect.height;
        float totalButtonHeight = (buttonHeight + verticalGap) * files.Length - verticalGap; // Calculate total height of all buttons
        float totalButtonWidth = buttonWidth; // Buttons are same width, so total width is constant

        float xOffset = (panelWidth - totalButtonWidth) / 2f; // Calculate the horizontal offset to center buttons
        float yOffset = (panelHeight - totalButtonHeight) / 2f; // Calculate the vertical offset to center buttons

        GameObject lastButton = null;

        foreach (string file in files)
        {
            GameObject fileObject = Instantiate(fileObjectPrefab, Vector3.zero, Quaternion.identity);
            fileObject.transform.SetParent(spawnPoint, false); // Ensure local position is correctly set
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileObject.GetComponentInChildren<TextMeshProUGUI>().text = fileName;

            // Set the size of the button
            RectTransform fileObjectRectTransform = fileObject.GetComponent<RectTransform>();
            fileObjectRectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);

            // Set button position
            if (lastButton == null)
            {
                // First button
                fileObjectRectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
            }
            else
            {
                // Subsequent buttons
                RectTransform lastButtonRectTransform = lastButton.GetComponent<RectTransform>();
                float newY = lastButtonRectTransform.anchoredPosition.y - buttonHeight - verticalGap;
                fileObjectRectTransform.anchoredPosition = new Vector2(xOffset, newY);
            }

            // Instantiate delete button
            GameObject deleteButton = Instantiate(deleteButtonPrefab, fileObject.transform);
            RectTransform deleteButtonRectTransform = deleteButton.GetComponent<RectTransform>();
            deleteButtonRectTransform.anchorMin = new Vector2(1, 0.5f); // Align to right
            deleteButtonRectTransform.anchorMax = new Vector2(1, 0.5f); // Align to right
            deleteButtonRectTransform.pivot = new Vector2(1, 0.5f); // Align to right
            deleteButtonRectTransform.anchoredPosition = new Vector2(buttonWidth / 2 + 15f, 0); // Set delete button position
            deleteButtonRectTransform.sizeDelta = new Vector2(buttonHeight, buttonHeight); // Make the delete button square

            Button deleteButtonComponent = deleteButton.GetComponent<Button>();
            deleteButtonComponent.onClick.AddListener(() => DeleteFile(file));

            Button button = fileObject.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => LoadNotepadScene(file));

            lastButton = fileObject;
        }
    }

    void DeleteFile(string filePath)
    {
        File.Delete(filePath);
        // Reload the scene or update the file list
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void backToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    void createFileAction()
    {
        SceneManager.LoadScene("Notepad");
    }

    void LoadNotepadScene(string path)
    {
        var fileContent = File.ReadAllBytes(path);
        var fileContentString = System.Text.Encoding.UTF8.GetString(fileContent);
        PlayerPrefs.SetString("fileContentString", fileContentString);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Notepad");
    }

    void SortByNameFunction()
    {
        Debug.Log("Sort by name");
        // Sort the buttons by name
        List<Transform> sortedButtons = new List<Transform>();
        foreach (Transform button in spawnPoint)
        {
            sortedButtons.Add(button);
        }
        sortedButtons.Sort((a, b) => string.Compare(a.GetComponentInChildren<TextMeshProUGUI>().text, b.GetComponentInChildren<TextMeshProUGUI>().text));

        // Re-arrange the buttons
        for (int i = 0; i < sortedButtons.Count; i++)
        {
            sortedButtons[i].SetSiblingIndex(i);
        }
    }

    void SortByDateFunction()
    {
        Debug.Log("Sort by date");
        // Sort the buttons by creation date
        List<Transform> sortedButtons = new List<Transform>();
        foreach (Transform button in spawnPoint)
        {
            sortedButtons.Add(button);
        }
        sortedButtons.Sort((a, b) => File.GetLastWriteTime(GetFilePath(a)).CompareTo(File.GetLastWriteTime(GetFilePath(b))));

        // Re-arrange the buttons
        for (int i = 0; i < sortedButtons.Count; i++)
        {
            sortedButtons[i].SetSiblingIndex(i);
        }
    }

    string GetFilePath(Transform button)
    {
        // Extract the file path from the button's text
        string buttonText = button.GetComponentInChildren<TextMeshProUGUI>().text;
        return "Assets/users/" + username + "/" + buttonText + ".txt";
    }
}
