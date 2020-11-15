using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class AntiAliasingDrop : MonoBehaviour
{

    public GameObject level;
    public GameObject fastMode;

    private void Start()
    {
        GetComponent<Dropdown>().value = (int)Camera.main.gameObject.GetComponent<PostProcessLayer>().antialiasingMode;
    }

    public void ValueChange(int value)
    {
        Camera.main.gameObject.GetComponent<PostProcessLayer>().antialiasingMode = (PostProcessLayer.Antialiasing)value;
        if(value == 1)
        {
            level.SetActive(true);
        }
        else
        {
            level.SetActive(false);
        }
        if (value == 0)
        {
            fastMode.SetActive(true);
        }
        else
        {
            fastMode.SetActive(false);
        }
    }
}
