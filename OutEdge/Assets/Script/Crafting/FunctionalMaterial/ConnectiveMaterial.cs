using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConnectiveMaterial : MonoBehaviour
{
    public float breakForce;
    public float breakTorque;

    public List<GameObject> sons = new List<GameObject>();
    public GameObject centerparent;

    public void OnJointBreak(float breakForce)
    {
        foreach(Joint joint in GetComponents<Joint>())
        {
            if (!joint.connectedBody)
            {
                Destroy(joint);
            }
        }
        CenterObject.CreateCenter(gameObject,(GetComponent<CenterObject>() == null ? centerparent.GetComponent<CenterObject>() : GetComponent<CenterObject>()).pickable);
    }

    [ContextMenu("RefreshJoint")]
    public void RefreshJoint()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        foreach (Transform child in transform)
        {
            Joint joint = child.GetComponent<Joint>();
            if (joint != null)
            {
                Rigidbody temp = joint.connectedBody;
                joint.connectedBody = null;
                joint.connectedBody = temp;
            }
            if(child.GetComponent<Rigidbody>())
                child.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void LinkTarget()
    {
        try
        {
            if (!GetComponent<CenterObject>())
            {
                ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
                joint.targetRotation = Quaternion.Inverse(transform.rotation) * joint.connectedBody.transform.rotation;
                joint.targetPosition = transform.InverseTransformPoint(joint.connectedBody.transform.position);
            }
        }
        catch { }
    }

    private void OnCollisionEnter(Collision collision)
    {
        (GetComponent<CenterObject>() ?? centerparent.GetComponent<CenterObject>()).RefreshChunkImplement();
    }

    /*public Quaternion initialRotation = Quaternion.identity;
    public Vector3 initialPosition = Vector3.zero;

    bool memorized = false;

    public void Memorize()
    {
        initialRotation = transform.rotation;
        initialPosition = transform.position;
        memorized = true;
    }

    public void Restore()
    {
        if (memorized)
        {
            gameObject.SetActive(false);
            transform.rotation = initialRotation;
            transform.position = initialPosition;
            gameObject.SetActive(true);
        }
    }*/
}
