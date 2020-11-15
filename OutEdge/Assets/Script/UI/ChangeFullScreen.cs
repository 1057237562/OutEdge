using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFullScreen : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<Toggle>().isOn = Screen.fullScreen;
    }

    public void ValueChanged(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
}
