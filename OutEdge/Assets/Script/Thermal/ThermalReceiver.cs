using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalReceiver : MonoBehaviour
{
    public float wmk;
    public int temperature = ThermalObject.global_tem;

    public List<ThermalObject> thermals = new List<ThermalObject>();
    Collider bc;

    private void Start()
    {
        bc = GetComponent<Collider>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        int tt = ThermalObject.global_tem;
        foreach(ThermalObject thermal in thermals)
        {
            tt += thermal.getTemperature(transform.position+bc.bounds.center,wmk);
        }
        temperature = tt;
        thermals.Clear();
    }
}
