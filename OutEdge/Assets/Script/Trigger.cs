using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public bool occpied = false;
    private void OnTriggerEnter(Collider other)
    {
        occpied = true;
    }
}
