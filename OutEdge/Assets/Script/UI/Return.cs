using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Return : MonoBehaviour
{
    public GameObject tar;
    public GameObject th;
    public void Action()
    {
        th.SetActive(false);
        try
        {
            tar.SetActive(true);
        }
        catch { }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
