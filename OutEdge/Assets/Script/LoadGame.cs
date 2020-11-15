using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    public GameObject loading;
    public GameObject start;
    public GameObject joining;

    public void click()
    {
        loading.SetActive(true);
        start.SetActive(false);
        joining.SetActive(false);
    }
}
