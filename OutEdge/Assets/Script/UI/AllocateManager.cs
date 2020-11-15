using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AllocateManager : MonoBehaviour
{
    public List<Toggle> items = new List<Toggle>();

    public static AllocateManager m;

    private void Awake()
    {
        m = this;

        if (File.Exists(Environment.CurrentDirectory + "/graphicssettings"))
        {
            bool[] vs = FileSystem.DeserializeFromFile<bool[]>(Environment.CurrentDirectory + "/graphicssettings");
            for(int i = 0; i< vs.Length; i++)
            {
                items[i].isOn = vs[i];
            }
        }
    }

    public void SaveData()
    {
        bool[] vs = new bool[items.Count];
        for(int i = 0; i < items.Count; i++)
        {
            vs[i] = items[i].isOn;
        }

        Thread thread = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
        thread.Start(new FileSystem.Values(vs, Environment.CurrentDirectory + "/graphicssettings"));
    }

}
