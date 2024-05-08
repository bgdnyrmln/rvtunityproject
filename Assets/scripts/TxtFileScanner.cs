using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class TxtFileScanner : MonoBehaviour
{
    // Static variable to store the current username.
    public static string username = "UnknownUser";

    // Prefab for file object.
    public GameObject fileObjectPrefab;

    // Transform to spawn file objects.
    public Transform spawnPoint;

    // Buttons for navigation and actions.
    public Button backToLogingScreen;
    public Button createFile;
    public Button SortByName;
    public Button SortByDate;
    public TMP_InputField searchInput;
    public Button searchButton;
    public GameObject createNewButton;
    public GameObject placeHolder;
    public Button NextPageButton;
    public Button PreviousPageButton;

    // Dimensions and layout parameters for file buttons.
    public float buttonWidth = 100f;
    public float buttonHeight = 60f;
    public float verticalGap = 15f;

    // Prefab for delete button.
    public GameObject deleteButtonPrefab;

    // Variables for pagination.
    private int currentPage = 0;
    private int filesPerPage = 6;
    private int totalFilesCount = 0;
    private string[] files;

    // Variables for search functionality.
    private string lastSearchText = "";
    private List<GameObject> fileObjects = new List<GameObject>();

    void Start()
    {
        // Add listeners to buttons.
        backToLogingScreen.onClick.AddListener(backToLogin);
        createFile.onClick.AddListener(createFileAction);
        SortByName.onClick.AddListener(SortByNameFunction);
        SortByDate.onClick.AddListener(SortByDateFunction);
        searchButton.onClick.AddListener(SearchFiles);
        NextPageButton.onClick.AddListener(NextPage);
        PreviousPageButton.onClick.AddListener(PreviousPage);

        // Get the folder path for user's files.
        string folderPath = Application.dataPath + "/users/" + username;

        // Scan and display files in the folder.
        ScanTxtFiles(folderPath);

        // Update pagination controls.
        UpdatePagination();
    }

    // Update button positions based on scroll value.
    void UpdateButtonPositions(float value)
    {
        // Calculate scrollable area and offset.
        float panelHeight = spawnPoint.GetComponent<RectTransform>().rect.height;
        float totalButtonHeight = (buttonHeight + verticalGap) * fileObjects.Count - verticalGap;
        float scrollableHeight = totalButtonHeight - panelHeight;
        float yOffset = scrollableHeight * value;

        // Update position of each file button.
        foreach (GameObject fileObject in fileObjects)
        {
            RectTransform fileObjectRectTransform = fileObject.GetComponent<RectTransform>();
            float xPos = (spawnPoint.GetComponent<RectTransform>().rect.width - buttonWidth) / 2f;
            float yPos = yOffset;
            fileObjectRectTransform.anchoredPosition = new Vector2(xPos, yPos);
            yOffset -= buttonHeight + verticalGap;
            fileObjectRectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        }
    }

    // Scan text files in the given folder.
    void ScanTxtFiles(string path, string searchText = "")
    {
        // Clear previously displayed files.
        foreach (Transform child in spawnPoint)
        {
            Destroy(child.gameObject);
        }
        fileObjects.Clear();

        // Check if the folder exists.
        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist: " + path);
            createNewButton.SetActive(true);
            UpdateButtonPositions(0f);
            return;
        }

        // Get all text files in the folder.
        files = Directory.GetFiles(path, "*.txt");

        // Check if there are no files.
        if (files.Length == 0)
        {
            createNewButton.SetActive(true);
            UpdateButtonPositions(0f);
            return;
        }

        // Filter files based on search text.
        if (!string.IsNullOrEmpty(searchText))
        {
            files = Array.FindAll(files, file => Path.GetFileNameWithoutExtension(file).ToLower().Contains(searchText.ToLower()));
        }

        // Sort files alphabetically by default.
        Array.Sort(files, (a, b) => string.Compare(Path.GetFileNameWithoutExtension(a), Path.GetFileNameWithoutExtension(b)));

        // Calculate total files count and update pagination.
        totalFilesCount = files.Length;
        UpdatePagination();

        // Calculate start and end indices for current page.
        int startIndex = currentPage * filesPerPage;
        int endIndex = Mathf.Min(startIndex + filesPerPage, totalFilesCount);

        // Instantiate file objects and delete buttons.
        for (int i = startIndex; i < endIndex; i++)
        {
            string file = files[i];
            GameObject fileObject = Instantiate(fileObjectPrefab, Vector3.zero, Quaternion.identity);
            fileObject.transform.SetParent(spawnPoint, false);
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileObject.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
            fileObjects.Add(fileObject);

            // Instantiate delete button.
            GameObject deleteButton = Instantiate(deleteButtonPrefab, fileObject.transform);
            RectTransform deleteButtonRectTransform = deleteButton.GetComponent<RectTransform>();
            deleteButtonRectTransform.anchorMin = new Vector2(1, 0.5f);
            deleteButtonRectTransform.anchorMax = new Vector2(1, 0.5f);
            deleteButtonRectTransform.pivot = new Vector2(1, 0.5f);
            deleteButtonRectTransform.anchoredPosition = new Vector2(buttonWidth / 2 + 15f, 0);
            deleteButtonRectTransform.sizeDelta = new Vector2(buttonHeight, buttonHeight);

            // Add listener to delete button.
            Button deleteButtonComponent = deleteButton.GetComponent<Button>();
            deleteButtonComponent.onClick.AddListener(() => DeleteFile(file));

            // Add listener to file object button.
            Button button = fileObject.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => LoadNotepadScene(file));
        }

        // Update button positions.
        UpdateButtonPositions(0f);
    }

    // Perform file search.
    void SearchFiles()
    {
        string folderPath = Application.dataPath + "/users/" + username;;
        string searchText = searchInput.text.Trim();
        lastSearchText = searchText;

        // Reset current page to 0 after search.
        currentPage = 0;

        // Scan files based on search text.
        ScanTxtFiles(folderPath, searchText);
    }

    // Update pagination controls based on total files count.
    void UpdatePagination()
    {
        int totalPages = Mathf.CeilToInt((float)totalFilesCount / filesPerPage);
        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);
        UpdateDisplayedFiles();
    }

    // Update displayed files based on current page.
    void UpdateDisplayedFiles()
    {
        // Clear previously displayed files.
        foreach (Transform child in spawnPoint)
        {
            Destroy(child.gameObject);
        }
        fileObjects.Clear();

        // Calculate start and end indices for current page.
        int startIndex = currentPage * filesPerPage;
        int endIndex = Mathf.Min(startIndex + filesPerPage, totalFilesCount);

        // Instantiate file objects and delete buttons.
        for (int i = startIndex; i < endIndex; i++)
        {
            string file = files[i];
            GameObject fileObject = Instantiate(fileObjectPrefab, Vector3.zero, Quaternion.identity);
            fileObject.transform.SetParent(spawnPoint, false);
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileObject.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
            fileObjects.Add(fileObject);

            // Instantiate delete button.
            GameObject deleteButton = Instantiate(deleteButtonPrefab, fileObject.transform);
            RectTransform deleteButtonRectTransform = deleteButton.GetComponent<RectTransform>();
            deleteButtonRectTransform.anchorMin = new Vector2(1, 0.5f);
            deleteButtonRectTransform.anchorMax = new Vector2(1, 0.5f);
            deleteButtonRectTransform.pivot = new Vector2(1, 0.5f);
            deleteButtonRectTransform.anchoredPosition = new Vector2(buttonWidth / 2 + 15f, 0);
            deleteButtonRectTransform.sizeDelta = new Vector2(buttonHeight, buttonHeight);

            // Add listener to delete button.
            Button deleteButtonComponent = deleteButton.GetComponent<Button>();
            deleteButtonComponent.onClick.AddListener(() => DeleteFile(file));

            // Add listener to file object button.
            Button button = fileObject.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => LoadNotepadScene(file));
        }
    }

    // Delete the selected file.
    void DeleteFile(string filePath)
    {
        File.Delete(filePath);
        string folderPath = Application.dataPath + "/users/" + username;
        ScanTxtFiles(folderPath, lastSearchText);
        UpdatePagination();
    } 

    // Load the login scene.
    void backToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    // Create a new file and load the notepad scene.
    void createFileAction()
    {
        PlayerPrefs.SetString("fileContentString", "");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Notepad");  
    }

    // Load the notepad scene with the selected file.
    void LoadNotepadScene(string path)
    {
        var fileContent = File.ReadAllBytes(path);
        var fileContentString = System.Text.Encoding.UTF8.GetString(fileContent);
        PlayerPrefs.SetString("fileContentString", fileContentString);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Notepad");
    }

    // Sort files alphabetically by name.
    void SortByNameFunction()
    {
        Array.Sort(files, (a, b) => string.Compare(Path.GetFileNameWithoutExtension(a), Path.GetFileNameWithoutExtension(b)));
        UpdatePagination();
    }

    // Sort files by last modified date.
    void SortByDateFunction()
    {
        Array.Sort(files, (a, b) => File.GetLastWriteTime(a).CompareTo(File.GetLastWriteTime(b)));
        UpdatePagination();
    }

    // Go to the next page.
    void NextPage()
    {
        currentPage++;
        UpdatePagination();
    }

    // Go to the previous page.
    void PreviousPage()
    {
        currentPage--;
        UpdatePagination();
    }
}
