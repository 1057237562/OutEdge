using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ChatController : MonoBehaviour {


    public TMP_InputField TMP_Chatinput;

    public TMP_Text TMP_ChatOutput;

    public Scrollbar ChatScrollbar;

    void OnEnable()
    {
        TMP_Chatinput.onSubmit.AddListener(AddToChatOutput);

    }

    void OnDisable()
    {
        TMP_Chatinput.onSubmit.RemoveListener(AddToChatOutput);

    }


    void AddToChatOutput(string newText)
    {
        // Clear input Field
        TMP_Chatinput.text = string.Empty;

        var timeNow = System.DateTime.Now;

        TMP_ChatOutput.text += "[<#FFFF80>" + timeNow.Hour.ToString("d2") + ":" + timeNow.Minute.ToString("d2") + ":" + timeNow.Second.ToString("d2") + "</color>] " + newText + "\n";

        TMP_Chatinput.ActivateInputField();

        // Set the scrollbar to the bottom when next text is submitted.
        ChatScrollbar.value = 0;

    }

}
