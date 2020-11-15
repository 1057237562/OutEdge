using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileDetection : MonoBehaviour
{
    public GameObject item;

    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo root = new DirectoryInfo(Environment.CurrentDirectory + "/saves/");
        DirectoryInfo[] dics = root.GetDirectories();

        foreach(DirectoryInfo dic in dics)
        {
            GameObject n_item = Instantiate(item, item.transform.parent);
            n_item.transform.GetChild(0).GetComponent<Text>().text = dic.Name;
            n_item.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
