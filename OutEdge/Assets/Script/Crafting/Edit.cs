using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Edit : MonoBehaviour
{
    public int tid;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { 
            Crafting.sid = tid; 
            Crafting.tid = tid;
            enabled = true;
            GetComponent<Image>().color = Color.gray;
            if (tid != -6)
                Crafting.m.ReleaseCurrent();
        });
        enabled = false;
    }

    private void Update()
    {
        if(Crafting.sid != tid)
        {
            enabled = false;
            GetComponent<Image>().color = Color.white;
        }
    }
}
