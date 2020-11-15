using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using static UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController;

public class AntiAnliasingToggle : MonoBehaviour
{
    public Toggle conflict;

    private void Start()
    {
        GetComponent<Toggle>().isOn = rfpc.cam.GetComponent<PostProcessLayer>().antialiasingMode != PostProcessLayer.Antialiasing.None;
    }

    public void Toggle(bool active)
    {
        if (!active)
        {
            rfpc.cam.GetComponent<PostProcessLayer>().antialiasingMode = PostProcessLayer.Antialiasing.None;
        }
        
    }

    public void Conflict(bool active)
    {
        if (active)
        {
            conflict.isOn = false;
        }
    }
}
