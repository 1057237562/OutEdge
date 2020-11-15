using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inspector : MonoBehaviour
{
    public List<GameObject> items;
    public List<string> iname;
    public List<string> description;

    public GameObject hoverboard;
    public Text oname;
    public Text odes;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in items)
        {
            MouseHoverListener mouseHover = obj.AddComponent<MouseHoverListener>();
            mouseHover.enter.AddListener(ShowBoard);
            mouseHover.exit.AddListener(HideBoard);
        }
    }

    public void ShowBoard(GameObject obj)
    {
        if(obj.transform.position.y > Screen.height - 100)
        {
            (hoverboard.transform as RectTransform).pivot = new Vector2(0, 1);
        }
        else if (obj.transform.position.y < 75)
        {
            (hoverboard.transform as RectTransform).pivot = new Vector2(0, 0);
        }
        else{
            (hoverboard.transform as RectTransform).pivot = new Vector2(0, 0.5f);
        }
        (hoverboard.transform as RectTransform).anchoredPosition = new Vector2(125, obj.transform.position.y);
        int index = items.IndexOf(obj);
        oname.text = iname[index];
        odes.text = description[index];
        hoverboard.SetActive(true);
    }

    public void HideBoard(GameObject obj)
    {
        hoverboard.SetActive(false);
    }
}
