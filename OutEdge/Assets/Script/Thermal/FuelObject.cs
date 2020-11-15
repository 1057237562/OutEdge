using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelObject : MonoBehaviour
{
    public int burntime;
    public int burnt_time = 0;
    public bool ignited = false;
    public GameObject particals;

    [SerializeField]
    public Ignite ignite;

    [Serializable]
    public class Ignite
    {
        public int temperature;
        public int type = -1;
    };

    public ThermalReceiver tr;

    public void FixedUpdate()
    {
        if (ignited)
        {
            if (Time.time*500 - burnt_time >= burntime)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (ignite.type != -1 && ignite.temperature < tr.temperature)
            {
                ThermalObject to = gameObject.AddComponent<ThermalObject>();
                to.temperature = ignite.type;
                ignited = true;
                particals.SetActive(true);
                Destroy(GetComponent<ThermalReceiver>());
                burnt_time = (int)(Time.time*500);
            }
        }
    }
}
