using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button registerButton;
    public Button goToLoginButton;
    public Button togglePasswordVisibilityButton;
    public Text logtext = null;

    public uint qsize = 1;

    ArrayList credentials;
    bool usernameInvalid = false;
    bool passwordInvalid = false;

    void Start()
    {
        registerButton.onClick.AddListener(ValidateAndRegister);
        goToLoginButton.onClick.AddListener(GoToLoginScene);
        togglePasswordVisibilityButton.onClick.AddListener(TogglePasswordVisibility);

        if (File.Exists(Application.dataPath + "/credentials.csv"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.csv"));
        }
        else
        {
            File.WriteAllText(Application.dataPath + "/credentials.csv", "");
        }
    }

    void GoToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }

    void TogglePasswordVisibility()
    {
        passwordInput.contentType = passwordInput.contentType == InputField.ContentType.Password ?
            InputField.ContentType.Standard : InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
        passwordInput.text = passwordInput.text;
    }

    void ValidateAndRegister()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        // Reset invalid flags
        usernameInvalid = false;
        passwordInvalid = false;

        if (!IsUsernameValid(username))
        {
            usernameInvalid = true;
        }

        if (!IsPasswordValid(password))
        {
            passwordInvalid = true;
        }

        if (!usernameInvalid && !passwordInvalid)
        {
            RegisterUser(username, password);
        }
        else
        {
            if (usernameInvalid)
            {
                Debug.Log("Invalid username.");
            }

            if (passwordInvalid)
            {
                Debug.Log("Invalid password.");
            }
        }
    }

    bool IsUsernameValid(string username)
    {
        // Username should not contain special symbols
        return !string.IsNullOrWhiteSpace(username) && !ContainsSpecialSymbols(username);
    }

    bool IsPasswordValid(string password)
    {
        // Password should be at least 8 characters long and contain at least one uppercase letter
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }
        else if (password.Length < 8 || !ContainsUpperCase(password))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool ContainsSpecialSymbols(string str)
    {
        // Check if the string contains any special symbols
        foreach (char c in str)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return true;
            }
        }
        return false;
    }

    bool ContainsUpperCase(string str)
    {
        // Check if the string contains at least one uppercase letter
        foreach (char c in str)
        {
            if (char.IsUpper(c))
            {
                return true;
            }
        }
        return false;
    }

    void RegisterUser(string username, string password)
    {
        bool isExists = credentials.Contains(username);
        if (isExists)
        {
            Debug.Log($"Username '{username}' already exists");
        }
        else
        {
            try
            {
                // Hash the password
                string hashedPassword = HashPassword(password);

                credentials.Add(username);
                // Save the hashed username and password
                File.AppendAllText(Application.dataPath + "/credentials.csv", username + "," + hashedPassword + "\n");
                Directory.CreateDirectory(Application.dataPath + "/users/" + username);
                Debug.Log("Account Registered");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
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

    // Log handling methods...

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
