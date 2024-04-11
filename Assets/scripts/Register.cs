using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    // number of messages to keep
    public uint qsize = 1;

    


    ArrayList credentials;

    // Start is called before the first frame update
    void Start()
    {
        registerButton.onClick.AddListener(writeStuffToFile);
        goToLoginButton.onClick.AddListener(goToLoginScene);
        togglePasswordVisibilityButton.onClick.AddListener(TogglePasswordVisibility);

        if (File.Exists(Application.dataPath + "/credentials.txt"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));
        }
        else
        {
            File.WriteAllText(Application.dataPath + "/credentials.txt", "");
        }

    }

    void goToLoginScene()
    {
        SceneManager.LoadScene("Login");
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

    void writeStuffToFile()
    {
        bool isExists = false;

        credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));
        foreach (var i in credentials)
        {
            if (i.ToString().Contains(usernameInput.text))
            {
                isExists = true;
                break;
            }
        }

        if (isExists)
        {
            Debug.Log($"Username '{usernameInput.text}' already exists");
        }
        else
        {
            try{
                // Write the new credentials to the file
                credentials.Add(usernameInput.text + ":" + passwordInput.text);
                File.WriteAllLines(Application.dataPath + "/credentials.txt", (String[])credentials.ToArray(typeof(string)));
                // Generate a unique user directory
                Directory.CreateDirectory(Application.dataPath + "/users/" + usernameInput.text);
                Debug.Log("Account Registered");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            
        }
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
