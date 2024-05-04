using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        loginButton.onClick.AddListener(login);
        goToRegisterButton.onClick.AddListener(moveToRegister);
        togglePasswordVisibilityButton.onClick.AddListener(TogglePasswordVisibility);

        if (File.Exists(Application.dataPath + "/credentials.csv"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.csv"));
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

    void login()
    {
        bool isAuthenticated = false;

        // Check if the credentials file exists
        if (File.Exists(Application.dataPath + "/credentials.csv"))
        {
            // Read all lines from the credentials file
            ArrayList credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.csv"));

            // Iterate through each line in the file
            foreach (var credential in credentials)
            {
                string line = credential.ToString();
                int delimiterIndex = line.IndexOf(",");

                // If the delimiter exists and not at the end of the line
                if (delimiterIndex != -1 && delimiterIndex < line.Length - 1)
                {
                    // Extract stored username and hashed password
                    string storedUsername = line.Substring(0, delimiterIndex);
                    string storedHashedPassword = line.Substring(delimiterIndex + 1);

                    // Hash the password entered by the user
                    string enteredHashedPassword = HashPassword(passwordInput.text);

                    // Compare stored hashed password with entered hashed password
                    if (storedUsername.Equals(usernameInput.text) && storedHashedPassword.Equals(enteredHashedPassword))
                    {
                        isAuthenticated = true;
                        break; // Exit the loop if authenticated
                    }
                }
            }
        }
        else
        {
            Debug.Log("Credential file doesn't exist");
        }

        if (isAuthenticated)
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

    string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            // Compute hash from the password string
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert byte array to a string representation
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                builder.Append(hashedBytes[i].ToString("x2"));
            }
            return builder.ToString();
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

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue(logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    void Update()
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
