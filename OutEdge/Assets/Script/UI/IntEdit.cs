using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntEdit : MonoBehaviour
{
    public Slider connected;

    public void UpdContent()
    {
        GetComponent<InputField>().text = connected.value.ToString();
    }

    public void EndEdit()
    {
        connected.value = float.Parse(GetComponent<InputField>().text);
    }
}
