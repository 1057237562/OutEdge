using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyResolution : MonoBehaviour
{
    public InputField width;
    public InputField height;

    public void Apply()
    {
        Screen.SetResolution(int.Parse(width.text), int.Parse(height.text), Screen.fullScreenMode, Screen.currentResolution.refreshRate);
    }
}
