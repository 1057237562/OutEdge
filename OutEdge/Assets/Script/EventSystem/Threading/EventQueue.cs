using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventQueue
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

    private Dictionary<object,Process> processes = new Dictionary<object,Process>();

    public void AddQueue(Action action,object index)
    {
        if(!processes.ContainsKey(index))
            processes.Add(index,new Process(action));
    }

    public void RunQueue()
    {
        foreach (Process process in processes.Values)
        {
            try
            {
                process.act();
            }
            catch { }
        }
        processes.Clear();
    }
}
