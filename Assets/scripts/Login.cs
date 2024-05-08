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
    // Input fields and buttons for the login UI.
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    public Button togglePasswordVisibilityButton;
    public GameObject fileSelectionPanel;
    public Button openExistingFileButton;
    public Button skipButton;
    public Text logtext = null;
    public uint qsize = 1; // Queue size for log messages.
    ArrayList credentials; // List to store credentials from the CSV file.

    // This function is called when the script starts.
    void Start()
    {
        // Adding listeners to buttons.
        loginButton.onClick.AddListener(login);
        goToRegisterButton.onClick.AddListener(moveToRegister);
        togglePasswordVisibilityButton.onClick.AddListener(TogglePasswordVisibility);

        // Checking if the credentials file exists.
        if (File.Exists(Application.dataPath + "/credentials.csv"))
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.csv"));
        else
            File.Create(Application.dataPath + "/credentials.csv");

        // Initially hiding the file selection panel.
        fileSelectionPanel.SetActive(false);

        // Adding listeners to file selection buttons.
        openExistingFileButton.onClick.AddListener(OpenExistingFile);
        skipButton.onClick.AddListener(SkipFileSelection);
    }

    // Function to handle login button click.
    void login()
    {
        bool isAuthenticated = false;

        // Checking if the credentials file exists.
        if (File.Exists(Application.dataPath + "/credentials.csv"))
        {
            // Reading all lines from the credentials file.
            ArrayList credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.csv"));

            // Iterating through each line in the file.
            foreach (var credential in credentials)
            {
                string line = credential.ToString();
                int delimiterIndex = line.IndexOf(",");

                // If the delimiter exists and not at the end of the line.
                if (delimiterIndex != -1 && delimiterIndex < line.Length - 1)
                {
                    // Extracting stored username and hashed password.
                    string storedUsername = line.Substring(0, delimiterIndex);
                    string storedHashedPassword = line.Substring(delimiterIndex + 1);

                    // Hashing the password entered by the user.
                    string enteredHashedPassword = HashPassword(passwordInput.text);

                    // Comparing stored hashed password with entered hashed password.
                    if (storedUsername.Equals(usernameInput.text) && storedHashedPassword.Equals(enteredHashedPassword))
                    {
                        isAuthenticated = true;
                        break; // Exiting the loop if authenticated.
                    }
                }
            }
        }
        else
        {
            Debug.Log("Credential file doesn't exist");
        }

        // Handling authentication result.
        if (isAuthenticated)
        {
            Debug.Log($"Logging in '{usernameInput.text}'");
            PlayerPrefs.SetString("Username", usernameInput.text); // Saving username.
            fileSelectionPanel.SetActive(true); // Showing file selection panel.
        }
        else
        {
            Debug.Log("Incorrect credentials");
        }
    }

    // Function to hash the password.
    string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            // Computing hash from the password string.
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Converting byte array to a string representation.
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                builder.Append(hashedBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Function to toggle password visibility.
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

    // Function to move to the registration scene.
    void moveToRegister()
    {
        SceneManager.LoadScene("Register");
    }

    // Function to open the existing file.
    void OpenExistingFile()
    {
        string username = PlayerPrefs.GetString("Username", "UnknownUser");
        TxtFileScanner.username = username;
        SceneManager.LoadScene("FileChoose");
    }

    // Function to skip file selection.
    void SkipFileSelection()
    {
        PlayerPrefs.SetString("fileContentString", "");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Notepad");
    }

    // Queue to store log messages.
    Queue myLogQueue = new Queue();

    // Function to enable logging.
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    // Function to disable logging.
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    // Function to handle log messages.
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue(logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    // Function to update log messages in the UI.
    void Update()
    {
        // Updating the text element with the latest log messages.
        string logText = "";
        foreach (string message in myLogQueue)
        {
            logText += message + "\n";
        }
        logtext.text = logText;
    }
}
