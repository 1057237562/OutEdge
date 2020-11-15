using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public static MessageBox m;
    public float startTime = -2f;

    public float stay = 2f;

    public GameObject[] messages;

    void Awake()
    {
        m = this; 
    }

    private void Update()
    {
        if(Time.fixedTime - startTime >= stay)
        {
            foreach (GameObject gobj in messages)
            {
                gobj.SetActive(false);
            }
        }
    }

    public static void ShowMessage(string message)
    {
        m.startTime = Time.fixedTime;
        foreach (GameObject gobj in m.messages)
        {
            gobj.SetActive(true);
            gobj.transform.GetChild(0).GetComponent<Text>().text = message;
        }
    }
}
