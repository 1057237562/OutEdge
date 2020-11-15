using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KeyCodeInput : MonoBehaviour
{

    bool recording = false;

    [Serializable]
    public class ValueChange : UnityEvent<string> {};

    [SerializeField]
    public ValueChange valueChange = new ValueChange();

    public void StartRecording()
    {
        if (recording)
        {
            valueChange.Invoke("Left Click");
            transform.GetChild(0).GetComponent<Text>().text = "Left Click";

            recording = false;
            return;
        }
        else
        {
            recording = true;
        }
    }

    void OnGUI()
    {
        if (recording)
        {
            if (Input.GetMouseButtonDown(1))
            {
                valueChange.Invoke("Right Click");
                transform.GetChild(0).GetComponent<Text>().text = "Right Click";

                recording = false;
                return;
            }
            if (Input.GetMouseButtonDown(2))
            {
                valueChange.Invoke("Middle Click");
                transform.GetChild(0).GetComponent<Text>().text = "Middle Click";

                recording = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                valueChange.Invoke("left ctrl");
                transform.GetChild(0).GetComponent<Text>().text = "left ctrl";

                recording = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                valueChange.Invoke("left shift");
                transform.GetChild(0).GetComponent<Text>().text = "left shift";

                recording = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                valueChange.Invoke("right ctrl");
                transform.GetChild(0).GetComponent<Text>().text = "right ctrl";

                recording = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                valueChange.Invoke("right shift");
                transform.GetChild(0).GetComponent<Text>().text = "right shift";

                recording = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                valueChange.Invoke("/");
                transform.GetChild(0).GetComponent<Text>().text = "/";

                recording = false;
                return;
            }
            if (Event.current.keyCode != KeyCode.None)
            {
                if (Event.current.keyCode.ToString().Length > 1)
                {
                    string output = Regex.Replace(Event.current.keyCode.ToString(), "[A-Z]", delegate (Match match)
                    {
                        string v = match.ToString();
                        return " " + char.ToLower(v[0]) + v.Substring(1);
                    }).Replace(" arrow", "").Replace("command", "cmd");

                    valueChange.Invoke(output.Trim());
                    transform.GetChild(0).GetComponent<Text>().text = output.Trim();
                }
                else
                {
                    valueChange.Invoke(Event.current.keyCode.ToString().ToLower());
                    transform.GetChild(0).GetComponent<Text>().text = Event.current.keyCode.ToString().ToLower();
                }

                recording = false;
                return;
            }
        }
    }

    public void SetKeyCode(string key)
    {
        valueChange.Invoke(key);
        transform.GetChild(0).GetComponent<Text>().text = key;
    }
}
