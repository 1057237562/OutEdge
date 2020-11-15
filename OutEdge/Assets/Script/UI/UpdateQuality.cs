using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class UpdateQuality : MonoBehaviour
{
    public PostProcessLayer post;

    private void Awake()
    {
        GetComponent<Dropdown>().value = (int)post.subpixelMorphologicalAntialiasing.quality;
    }

    public void ValueChange(int value)
    {
        post.subpixelMorphologicalAntialiasing.quality = (SubpixelMorphologicalAntialiasing.Quality)value;
    }
}
