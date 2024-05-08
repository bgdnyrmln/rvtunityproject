using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Variables to store log output and stack trace.
    public string output = "";
    public string stack = "";

    // Called when this script is enabled.
    void OnEnable()
    {
        // adds message to log
        Application.logMessageReceived += HandleLog;
    }

    // Called when this script is disabled.
    void OnDisable()
    {
        // deletes message from log
        Application.logMessageReceived -= HandleLog;
    }

    // Function to handle log messages.
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Store the log message and stack trace.
        output = logString;
        stack = stackTrace;
    }
}
