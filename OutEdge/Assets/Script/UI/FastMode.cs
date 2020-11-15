using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class FastMode : MonoBehaviour
{
    public PostProcessLayer post;

    private void Awake()
    {
        GetComponent<Toggle>().isOn = post.fastApproximateAntialiasing.fastMode;
    }

    public void Toggle(bool active)
    {
        post.fastApproximateAntialiasing.fastMode = active;
    }
}
