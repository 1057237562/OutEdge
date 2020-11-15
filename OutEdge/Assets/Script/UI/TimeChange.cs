using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChange : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.mainTextureOffset = new Vector2(1f / 24f * DateTime.Now.Hour, 0);
    }
}
