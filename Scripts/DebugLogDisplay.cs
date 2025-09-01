using TMPro;
using UnityEngine;

public class DebugLogDisplay : MonoBehaviour
{
    public TMP_Text debugText;
    private string logOutput = "";

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
        logOutput += logString + "\n";

        // Trim if too long
        if (logOutput.Length > 5000)
        {
            logOutput = logOutput.Substring(logOutput.Length - 5000);
        }

        if (debugText != null)
        {
            debugText.text = logOutput;
        }
    }
}