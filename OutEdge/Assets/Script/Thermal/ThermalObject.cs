using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalObject : MonoBehaviour
{
    public static int global_tem = 25;

    public bool heatSource = true;
    public int temperature;
    public int radius;
    public int slice = 24;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public struct Data
    {
        public float radius;
        public float wmk;

        public Data(float r,float k)
        {
            radius = r;
            wmk = k;
        }
    }

    public List<Data>[,] modifier;

    // Update is called once per frame
    void FixedUpdate()
    {
        int deltaTem = Mathf.Abs(temperature - global_tem);
        if (deltaTem > 2)
        {
            radius = (int)Mathf.Log(deltaTem, 2);
            //slice = (int)(Mathf.PI / Mathf.Asin(1f / (2f * radius))); // 2pi/2arcsin(1/2radius)   rid one of '2'
            modifier = new List<Data>[slice,slice];
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach(Collider co in colliders)
            {
                ThermalReceiver tr = co.GetComponent<ThermalReceiver>();
                if(tr != null)
                {
                    tr.thermals.Add(this);

                    Vector3 e = tr.transform.position + co.bounds.center - transform.position; // Based on X asix
                    int x = (int)((e.y > 0 ? slice / 2f : 0) + slice / 2f * ((Mathf.Atan(e.y / e.x) < 0 ? Mathf.PI : 0) + Mathf.Atan(e.y / e.x)) / Mathf.PI);
                    int y = (int)((e.y > 0 ? slice / 2f : 0) + slice / 2f * ((Mathf.Atan(e.z / e.y) < 0 ? Mathf.PI : 0) + Mathf.Atan(e.z / e.y)) / Mathf.PI);
                    try
                    {
                        if (modifier[x, y] == null)
                        {
                            modifier[x, y] = new List<Data>();
                        }
                        modifier[x, y].Add(new Data(e.magnitude, tr.wmk));
                    }
                    catch
                    {
                        Debug.LogError(x + ":" + y);
                    }
                }
            }
        }
        else
        {
            enabled = false;
        }
    }

    public void SetTemperature(int tar)
    {
        temperature = tar;
        enabled = true;
    }

    public int getTemperature(Vector3 pos,float m_wmk)
    {
        Vector3 e = pos - transform.position; // Based on X asix
        if(e.magnitude > radius)
        {
            return global_tem;
        }
        List<Data> piece = modifier[(int)((e.y > 0 ? slice / 2f : 0) + slice / 2f * ((Mathf.Atan(e.y / e.x) < 0 ? Mathf.PI : 0) + Mathf.Atan(e.y / e.x)) / Mathf.PI), (int)((e.y > 0 ? slice / 2f : 0) + slice / 2f * ((Mathf.Atan(e.z / e.y) < 0 ? Mathf.PI : 0) + Mathf.Atan(e.z / e.y)) / Mathf.PI)];
        if (piece == null)
        {
            return (int)(temperature * Mathf.Pow(0.5f, (int)e.magnitude - 1));
        }
        else
        {
            float[] tmp = new float[radius+1];
            int index = -1;
            foreach (Data data in piece)
            {
                if(data.radius > e.magnitude && data.wmk < m_wmk)
                {
                    index = (int)e.magnitude;
                }
                if (tmp[(int)data.radius] == 0 || tmp[(int)data.radius] > data.wmk)
                {
                    tmp[(int)data.radius] = data.wmk;
                }
            }
            float[] heat = new float[radius + 1];
            heat[0] = temperature;
            for (int i = 1; i <= radius; i++)
            {
                if(tmp[i] == 0)
                {
                    tmp[i] = 1;
                }
                heat[i] = tmp[i] * (heat[i - 1] - global_tem) / 2f + global_tem;
                if (index == i)
                {
                    return (int)(m_wmk * (heat[i - 1] - global_tem) / 2 + global_tem);
                }
                heat[i - 1] -= tmp[i] * (heat[i - 1] - global_tem) / 2f;
            }
            return (int)heat[(int)e.magnitude];
        }
    }
}
