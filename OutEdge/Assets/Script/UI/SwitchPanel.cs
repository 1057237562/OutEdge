using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPanel : MonoBehaviour
{
    public List<GameObject> panels = new List<GameObject>();

    public void SwitchTo(int index)
    {
        for(int i = 0; i < panels.Count; i++)
        {
            if(i == index)
            {
                panels[i].SetActive(true);
            }
            else
            {
                panels[i].SetActive(false);
            }
        }
    }
}
