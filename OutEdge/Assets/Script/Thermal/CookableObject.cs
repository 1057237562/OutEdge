using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemManager;

[RequireComponent(typeof(ThermalReceiver))]
public class CookableObject : MonoBehaviour
{
    public ThermalReceiver tr;
    public int minTemperature;
    public int maxTemperature;
    public Item output;
    public int time;
    int starttime;

    private void Start()
    {
        tr = GetComponent<ThermalReceiver>();
    }

    private void Update()
    {
        if(tr.temperature >= minTemperature && tr.temperature <= maxTemperature)
        {
            if(starttime == 0)
            {
                starttime = (int)(Time.time*1000);
            }else if(Time.time * 1000 - starttime >= time)
            {
                SummonItem(transform.position, output);
                Destroy(gameObject);
            }
        }
    }
}
