using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float length = 10.0f;

    public float attractive = 1.0f;

    void OnTriggerStay(Collider collider)
    {
        if(collider.GetComponent<Rigidbody>() == null || collider.gameObject.layer != LayerMask.NameToLayer("Default"))
        {
            return;
        }
        try
        {
            Vector3 direction = (collider.transform.position+transform.position)/2 - transform.position;
            float ratio = direction.magnitude / length;

            transform.parent.GetComponent<Rigidbody>().AddRelativeForce(direction * attractive / ratio);
            collider.GetComponent<Rigidbody>().AddRelativeForce(-direction * attractive / ratio);
        }
        catch { }
    }
}
