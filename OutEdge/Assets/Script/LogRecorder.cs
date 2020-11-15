using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LogRecorder : MonoBehaviour
{
    public StringBuilder stringBuilder = new StringBuilder();

    // Start is called before the first frame update
    private void Start()
    {
        Application.logMessageReceived += HandleUnityLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleUnityLog;
    }

    void HandleUnityLog(string message, string stack_trace, LogType type)
    {
        stringBuilder.Append(message +Environment.NewLine + stack_trace + Environment.NewLine);
    }

    private void OnDestroy()
    {
        File.WriteAllText(Environment.CurrentDirectory + "/DEBUG.log", stringBuilder.ToString());
    }
}
