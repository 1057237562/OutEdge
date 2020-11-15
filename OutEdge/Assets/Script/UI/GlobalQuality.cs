using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalQuality : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Dropdown>().value = QualitySettings.GetQualityLevel();
    }

    public void ValueChange(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }
}
