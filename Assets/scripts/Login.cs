using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;


public class Login : MonoBehaviour
{

    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    public Button togglePasswordVisibilityButton;
    public GameObject fileSelectionPanel;
    public Button openExistingFileButton;
    public Button skipButton;
    public Text logtext = null;
    public uint qsize = 1;
    ArrayList credentials;


    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(login);
        goToRegisterButton.onClick.AddListener(moveToRegister);
        togglePasswordVisibilityButton.onClick.AddListener(TogglePasswordVisibility);

        if (File.Exists(Application.dataPath + "/credentials.txt"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));
        }
        else
        {
            Debug.Log("Credential file doesn't exist");
        }
        fileSelectionPanel.SetActive(false); // Initially hide the file selection panel

        // Add listeners for file selection buttons
        openExistingFileButton.onClick.AddListener(OpenExistingFile);
        skipButton.onClick.AddListener(SkipFileSelection);
    }



    // Update is called once per frame
    void login()
{
    bool isExists = false;

    // Check if the credentials file exists
    if (File.Exists(Application.dataPath + "/credentials.txt"))
    {
        ArrayList credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));

        foreach (var i in credentials)
        {
            string line = i.ToString();
            int delimiterIndex = line.IndexOf(":");
            if (delimiterIndex != -1 && delimiterIndex < line.Length - 1)
            {
                string storedUsername = line.Substring(0, delimiterIndex);
                string storedPassword = line.Substring(delimiterIndex + 1);

                if (storedUsername.Equals(usernameInput.text) && storedPassword.Equals(passwordInput.text))
                {
                    isExists = true;
                    break;
                }
            }
        }
    }
    else
    {
        Debug.Log("Credential file doesn't exist");
    }

    if (isExists)
    {
        Debug.Log($"Logging in '{usernameInput.text}'");
        PlayerPrefs.SetString("Username", usernameInput.text); // Save username
        fileSelectionPanel.SetActive(true); // Show file selection panel
    }
    else
    {
        Debug.Log("Incorrect credentials");
    }
}

    void TogglePasswordVisibility()
    {
        if (passwordInput.contentType == InputField.ContentType.Password)
        {
            passwordInput.contentType = InputField.ContentType.Standard;
        }
        else
        {
            passwordInput.contentType = InputField.ContentType.Password;
        }

        passwordInput.ForceLabelUpdate();
        passwordInput.text = passwordInput.text;
    }

    void moveToRegister()
    {
        SceneManager.LoadScene("Register");
    }
    void OpenExistingFile()
    {
        string username = PlayerPrefs.GetString("Username", "UnknownUser");
        TxtFileScanner.username = username;
        SceneManager.LoadScene("FileChoose");
    }

    void SkipFileSelection()
    {
        PlayerPrefs.SetString("fileContentString", "");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Notepad");
    }


    Queue myLogQueue = new Queue();

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        myLogQueue.Enqueue(logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }


    void Update()  // Update is called every frame
    {
        // Update the text element with the latest log messages
        string logText = "";
        foreach (string message in myLogQueue)
        {
            logText += message + "\n";
        }
        logtext.text = logText;
    }

}