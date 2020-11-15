using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;
using static LoadControls;

public class SwitchControl : MonoBehaviour
{
    public bool axis = false;

    public void ChangeControl(string name)
    {
        if (axis)
        {
            if(loadControls.axistar.IndexOf(gameObject) % 2 == 0)
            {
                m.axispair[m.axisrefer.IndexOf(loadControls.axispair[loadControls.axistar.IndexOf(gameObject) / 2])].positive = name;
            }
            if (loadControls.axistar.IndexOf(gameObject) % 2 == 1)
            {
                m.axispair[m.axisrefer.IndexOf(loadControls.axispair[loadControls.axistar.IndexOf(gameObject) / 2])].negative = name;
            }
        }
        else
        {
            m.keypair[m.keyrefer.IndexOf(loadControls.keypair[loadControls.tar.IndexOf(gameObject)])] = name;
        }
        m.SaveControlSettings();
    }
}
