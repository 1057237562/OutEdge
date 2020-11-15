using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ResourceItem : MonoBehaviour
{
    public static float range = 32f;

    // Update is called once per frame
    void Update()
    {
        if((RigidbodyFirstPersonController.rfpc.transform.position - transform.position).magnitude > range)
        {
            Destroy(gameObject);
        }
    }
}
