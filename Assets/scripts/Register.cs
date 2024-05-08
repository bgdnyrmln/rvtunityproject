using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Register : MonoBehaviour
{
    // Input fields and buttons for user registration.
    public InputField usernameInput;
    public InputField passwordInput;
    public Button registerButton;
    public Button goToLoginButton;
    public Button togglePasswordVisibilityButton;
    public Text logtext = null;

    // Queue size for log messages.
    public uint qsize = 1;

    ArrayList credentials; // List to store registered usernames.
    bool usernameInvalid = false; // Flag to indicate invalid username.
    bool passwordInvalid = false; // Flag to indicate invalid password.

    void Start()
    {
        // Add listeners to buttons for their click events.
        registerButton.onClick.AddListener(ValidateAndRegister);
        goToLoginButton.onClick.AddListener(GoToLoginScene);
        togglePasswordVisibilityButton.onClick.AddListener(TogglePasswordVisibility);

        // Initialize credentials list.
        if (File.Exists(Application.dataPath + "/credentials.csv"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.csv"));
        }
        else
        {
            File.WriteAllText(Application.dataPath + "/credentials.csv", "");
        }
    }

    // Function to switch to the login scene.
    void GoToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }

    // Function to toggle password visibility.
    void TogglePasswordVisibility()
    {
        passwordInput.contentType = passwordInput.contentType == InputField.ContentType.Password ?
            InputField.ContentType.Standard : InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
        passwordInput.text = passwordInput.text;
    }

    // Function to validate input fields and register the user.
    void ValidateAndRegister()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        // Reset invalid flags
        usernameInvalid = false;
        passwordInvalid = false;

        // Validate username and password
        if (!IsUsernameValid(username))
        {
            usernameInvalid = true;
        }

        if (!IsPasswordValid(password))
        {
            passwordInvalid = true;
        }

        // Register user if input is valid
        if (!usernameInvalid && !passwordInvalid)
        {
            RegisterUser(username, password);
        }
        else
        {
            // Log error messages for invalid input
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

    // Function to check if username is valid.
    bool IsUsernameValid(string username)
    {
        return !string.IsNullOrWhiteSpace(username) && !ContainsSpecialSymbols(username) && IsValidUsernameFormat(username);
    }

    // Function to validate username format.
    bool IsValidUsernameFormat(string username)
    {
        bool hasLetter = false;
        bool hasNumber = false;

        foreach (char c in username)
        {
            if (char.IsLetter(c) && !char.IsUpper(c)) // Non-capital letter
            {
                hasLetter = true;
            }
            else if (char.IsDigit(c))
            {
                hasNumber = true;
            }
            else
            {
                return false; // Contains special symbol or capital letter
            }
        }

        return hasLetter && (hasNumber || !username.Any(char.IsDigit)); // Numbers not alone
    }

    // Function to check if password is valid.
    bool IsPasswordValid(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length >= 8 && ContainsUpperCase(password);
    }

    // Function to check if string contains special symbols.
    bool ContainsSpecialSymbols(string str)
    {
        foreach (char c in str)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return true;
            }
        }
        return false;
    }

    // Function to check if string contains uppercase letter.
    bool ContainsUpperCase(string str)
    {
        foreach (char c in str)
        {
            if (char.IsUpper(c))
            {
                return true;
            }
        }
        return false;
    }

    // Function to register the user.
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

                // Add username to credentials list
                credentials.Add(username);
                // Save username and hashed password to file
                File.AppendAllText(Application.dataPath + "/credentials.csv", username + "," + hashedPassword + "\n");
                // Create user directory
                Directory.CreateDirectory(Application.dataPath + "/users/" + username);
                Debug.Log("Account Registered");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    // Function to hash the password.
    string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                builder.Append(hashedBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Log handling methods...

    // Queue to store log messages.
    Queue myLogQueue = new Queue();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

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

    // Update is called every frame
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
