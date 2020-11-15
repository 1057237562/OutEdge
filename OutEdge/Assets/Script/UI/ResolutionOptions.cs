using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionOptions : MonoBehaviour
{
    List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

    public bool EqualRes(Resolution r1,Resolution r2)
    {
        return r1.width == r2.width && r1.height == r2.height && r2.refreshRate == r1.refreshRate;
    }

    private void Awake()
    {
        Resolution resolution;
        for (int i = 0 ; i < Screen.resolutions.Length; i++)
        {
            resolution = Screen.resolutions[i];
            if (EqualRes(resolution, Screen.currentResolution))
            {
                GetComponent<Dropdown>().value = i;
            }
            options.Add(new Dropdown.OptionData(resolution.width + "x" + resolution.height + "   " + resolution.refreshRate + "hz"));
        }

        GetComponent<Dropdown>().options = options;
    }

    public void ValueChanged(int id)
    {
        Screen.SetResolution(Screen.resolutions[id].width,Screen.resolutions[id].height,Screen.fullScreenMode,Screen.resolutions[id].refreshRate);
    }
}
