using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spite : MonoBehaviour
{
    public int destroydelay = 5;
    public GameObject tracker;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroydelay);
    }
}
