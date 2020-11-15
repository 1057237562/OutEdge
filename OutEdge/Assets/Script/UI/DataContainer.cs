using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContainer : MonoBehaviour
{

    public string data;

    public void ValueChange(object value)
    {
        data = value.ToString();
    }

    public void NumericalChange(float value)
    {
        data = value.ToString();
    }

    public void BooleanChange(bool value)
    {
        data = value.ToString();
    }

}
