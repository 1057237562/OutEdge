using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    public MeshRenderer main;
    public MeshRenderer sub;
    void Start()
    {
        if(System.DateTime.Now.Month == 4 && System.DateTime.Now.Day == 1)
        {
            MeshRenderer temp = main;
            main = sub;
            sub = temp;
        }
        if(Random.Range(1,1000) == 1){
            main.enabled = false;
            sub.enabled = true;
        }else
        {
            main.enabled = true;
            sub.enabled = false;
        }
    }
}
