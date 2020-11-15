using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class InputManager:MonoBehaviour
{

    [Serializable]
    public class Axis
    {
        public string positive;
        public string negative;
    }

    public List<string> keyrefer;
    public List<string> axisrefer;

    public List<Axis> axispair;

    public List<string> keypair;

    public static InputManager m;
    public Toggle autopick;

    private void Start()
    {
        m = this;
        LoadControlSettings();
        autopick.isOn = ItemBase.autopick;
    }

    public static int GetAxis(string name)
    {
        Axis axis = m.axispair[m.axisrefer.IndexOf(name)];
        if (Input.GetKey(axis.positive))
        {
            return 1;
        }
        if (Input.GetKey(axis.negative))
        {
            return -1;
        }
        return 0;
    }
    public static bool GetKeyUp(string name)
    {
        string key = m.keypair[m.keyrefer.IndexOf(name)];
        return Input.GetKeyUp(key);
    }
    public static bool GetKey(string name)
    {
        string key = m.keypair[m.keyrefer.IndexOf(name)];
        return Input.GetKey(key);
    }
    public static bool GetKeyDown(string name)
    {
        string key = m.keypair[m.keyrefer.IndexOf(name)];
        return Input.GetKeyDown(key);
    }

    public void SaveControlSettings()
    {
        Thread thread = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
        thread.Start(new FileSystem.Values(m.keypair, Environment.CurrentDirectory + "/keyset"));
        Thread thread2 = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
        thread2.Start(new FileSystem.Values(m.axispair, Environment.CurrentDirectory + "/axisset"));
    }

    public void LoadControlSettings()
    {
        if (File.Exists(Environment.CurrentDirectory + "/keyset"))
        {
            keypair = FileSystem.DeserializeFromFile<List<string>>(Environment.CurrentDirectory + "/keyset");
        }
        if (File.Exists(Environment.CurrentDirectory + "/axisset"))
        {
            axispair = FileSystem.DeserializeFromFile<List<Axis>>(Environment.CurrentDirectory + "/axisset");
        }
    }

    public void SetAutoPick(bool bo)
    {
        ItemBase.autopick = bo;
    }

}
