using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class TxtFileScanner : MonoBehaviour
{
    public static string username = "UnknownUser";
    public GameObject fileObjectPrefab;
    public Transform spawnPoint;
    public Button backToLogingScreen;
    public Button createFile;
    public Button SortByName;
    public Button SortByDate;
    public TMP_InputField searchInput;
    public Button searchButton;
    public GameObject createNewButton;
    public GameObject placeHolder;
    public float buttonWidth = 100f;
    public float buttonHeight = 60f;
    public float verticalGap = 15f;
    public Scrollbar scrollbar;
    public GameObject deleteButtonPrefab;

    private string lastSearchText = "";
    private List<GameObject> fileObjects = new List<GameObject>();

    void Start()
    {
        backToLogingScreen.onClick.AddListener(backToLogin);
        createFile.onClick.AddListener(createFileAction);
        SortByName.onClick.AddListener(SortByNameFunction);
        SortByDate.onClick.AddListener(SortByDateFunction);
        searchButton.onClick.AddListener(SearchFiles);
        scrollbar.onValueChanged.AddListener(UpdateButtonPositions);
        string folderPath = "Assets/users/" + username;
        ScanTxtFiles(folderPath);
    }

    void UpdateButtonPositions(float value)
    {
        float panelHeight = spawnPoint.GetComponent<RectTransform>().rect.height;
        float totalButtonHeight = (buttonHeight + verticalGap) * fileObjects.Count - verticalGap;
        float scrollableHeight = totalButtonHeight - panelHeight;

        float yOffset = scrollableHeight * value;

        foreach (GameObject fileObject in fileObjects)
        {
            RectTransform fileObjectRectTransform = fileObject.GetComponent<RectTransform>();

            float xPos = (spawnPoint.GetComponent<RectTransform>().rect.width - buttonWidth) / 2f;
            float yPos = yOffset;

            fileObjectRectTransform.anchoredPosition = new Vector2(xPos, yPos);
            yOffset -= buttonHeight + verticalGap;

            // Set button size
            fileObjectRectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        }
    }

    void Update()
    {
        UpdateButtonPositions(scrollbar.value);
    }

    void ScanTxtFiles(string path, string searchText = "")
    {
        foreach (Transform child in spawnPoint)
        {
            Destroy(child.gameObject);
        }

        fileObjects.Clear();

        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist: " + path);
            createNewButton.SetActive(true);
            UpdateButtonPositions(0f);
            return;
        }

        string[] files = Directory.GetFiles(path, "*.txt");
        if (files.Length == 0)
        {
            createNewButton.SetActive(true);
            UpdateButtonPositions(0f);
            return;
        }

        Array.Sort(files, (a, b) => string.Compare(Path.GetFileNameWithoutExtension(a), Path.GetFileNameWithoutExtension(b)));

        if (!string.IsNullOrEmpty(searchText))
        {
            files = Array.FindAll(files, file => Path.GetFileNameWithoutExtension(file).ToLower().Contains(searchText.ToLower()));
        }

        foreach (string file in files)
        {
            GameObject fileObject = Instantiate(fileObjectPrefab, Vector3.zero, Quaternion.identity);
            fileObject.transform.SetParent(spawnPoint, false);
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileObject.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
            fileObjects.Add(fileObject);

            // Instantiate delete button
            GameObject deleteButton = Instantiate(deleteButtonPrefab, fileObject.transform);
            RectTransform deleteButtonRectTransform = deleteButton.GetComponent<RectTransform>();
            deleteButtonRectTransform.anchorMin = new Vector2(1, 0.5f); // Align to right
            deleteButtonRectTransform.anchorMax = new Vector2(1, 0.5f); // Align to right
            deleteButtonRectTransform.pivot = new Vector2(1, 0.5f); // Align to right
            deleteButtonRectTransform.anchoredPosition = new Vector2(buttonWidth / 2 + 15f, 0); // Set delete button position
            deleteButtonRectTransform.sizeDelta = new Vector2(buttonHeight, buttonHeight); // Make the delete button square

            // Add listener to delete button
            Button deleteButtonComponent = deleteButton.GetComponent<Button>();
            deleteButtonComponent.onClick.AddListener(() => DeleteFile(file));

            // Add listener to file object button
            Button button = fileObject.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => LoadNotepadScene(file));
        }

        UpdateButtonPositions(scrollbar.value);
    }

    void SearchFiles()
    {
        string folderPath = "Assets/users/" + username;
        string searchText = searchInput.text.Trim();
        lastSearchText = searchText;
        ScanTxtFiles(folderPath, searchText);
    }

    void DeleteFile(string filePath)
    {
        File.Delete(filePath);
        string folderPath = "Assets/users/" + username;
        ScanTxtFiles(folderPath, lastSearchText);
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
        List<Transform> sortedButtons = new List<Transform>();
        foreach (Transform button in spawnPoint)
        {
            sortedButtons.Add(button);
        }
        sortedButtons.Sort((a, b) => string.Compare(a.GetComponentInChildren<TextMeshProUGUI>().text, b.GetComponentInChildren<TextMeshProUGUI>().text));

        for (int i = 0; i < sortedButtons.Count; i++)
        {
            sortedButtons[i].SetSiblingIndex(i);
        }
    }

    void SortByDateFunction()
    {
        List<Transform> sortedButtons = new List<Transform>();
        foreach (Transform button in spawnPoint)
        {
            sortedButtons.Add(button);
        }
        sortedButtons.Sort((a, b) => File.GetLastWriteTime(GetFilePath(a)).CompareTo(File.GetLastWriteTime(GetFilePath(b))));

        for (int i = 0; i < sortedButtons.Count; i++)
        {
            sortedButtons[i].SetSiblingIndex(i);
        }
    }

    string GetFilePath(Transform button)
    {
        string buttonText = button.GetComponentInChildren<TextMeshProUGUI>().text;
        return "Assets/users/" + username + "/" + buttonText + ".txt";
    }
}
