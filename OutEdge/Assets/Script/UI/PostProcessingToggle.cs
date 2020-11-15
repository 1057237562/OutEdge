using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PostProcessingToggle : MonoBehaviour
{
    public PostProcessProfile post;
    public int index = 0;

    private void Awake()
    {
        GetComponent<Toggle>().isOn = post.settings[index].active;
    }

    public void Toggle(bool active)
    {
        post.settings[index].active = active;
    }

}
