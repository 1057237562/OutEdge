using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Looper : MonoBehaviour
{
    class Process
    {
        public Action act;
        public bool done;

        public Process(Action action)
        {
            act = action;
            done = false;
        }
    }

    static List<Process> processes = new List<Process>();

    public static void Loop(Action action)
    {
        if (action != null)
        {
            Process process = new Process(action);
            processes.Add(process);
            while (!process.done)
            {
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        while(processes.Count > 0)
        {
            try
            {
                processes[0].act();
                processes[0].done = true;
            }
            catch
            {

            }

            processes.RemoveAt(0);
        }
    }
}
