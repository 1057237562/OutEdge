using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager
{

    public static float startTime;
    public static float lastPlayTime = 0;

    public static void SetLastPlayTime(float input)
    {
        lastPlayTime = input;
        startTime = Time.fixedTime;
    }

    public static float GetCurrentPlayTime()
    {
        return Time.fixedTime - startTime + lastPlayTime;
    }
}
