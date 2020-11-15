using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfScreen : MonoBehaviour
{
    public int upd = 0;
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width-40);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height / 2);
        rt.anchoredPosition = new Vector2(0 ,-(upd*Screen.height/2)-Screen.height/4);
    }
}
