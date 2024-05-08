using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script handles notepad loading.

public class loadinginto : MonoBehaviour
{
    public GameObject placeHolder; // Text input placeholder.
    public Button backToLogin; // Button to return to login scene.
    public Button backToFileChoose; // Button to return to file choosing scene.

    // Called when the game starts.
    void Start()
    {
        // Listen for click to return to login scene.
        backToLogin.onClick.AddListener(GoToLoginScene);
        // Listen for click to return to file choosing scene.
        backToFileChoose.onClick.AddListener(GoToFileScene);
        // Set any opened text to the input placeholder.
        string savedValue = PlayerPrefs.GetString("fileContentString", "");
        placeHolder.GetComponent<InputField>().text = savedValue;
    }

    // Return to the login scene.
    void GoToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }

    // Return to the file choosing scene.
    void GoToFileScene()
    {
        // Get the username from saved preferences.
        string username = PlayerPrefs.GetString("Username", "UnknownUser");
        // Set the username for the file scanner.
        TxtFileScanner.username = username;
        // Load the file choosing scene.
        SceneManager.LoadScene("FileChoose");
    }
}
