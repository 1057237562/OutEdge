using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GuiObject : MonoBehaviour
{

    public Camera gui;
    public bool interacting = false;
    public List<Action> interact = new List<Action>();
    public List<Action> lostfocus = new List<Action>();
    public bool CanDestroy = true;

    void Start(){
        try
        {
            if (gui != null)
            {
                if (QualitySettings.GetQualityLevel() == 0)
                {
                    gui.GetComponent<PostProcessLayer>().enabled = false;
                }
                else
                {
                    gui.GetComponent<PostProcessLayer>().enabled = true;
                }
            }
        }
        catch { }
    }

    public void Interact()
    {
        foreach(Action a in interact)
        {
            a();
        }
    }

    public void LostFocus()
    {
        foreach (Action a in lostfocus)
        {
            a();
        }
    }
}
