using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public GameObject tar;
    public GameObject th;
    public void Action()
    {
        tar.SetActive(true);
        th.SetActive(false);
    }
}
