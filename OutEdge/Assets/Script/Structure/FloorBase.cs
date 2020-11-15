using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBase : StructureEntity
{
    public GameObject basement;
    private void OnEnable()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        basement.layer = LayerMask.NameToLayer("BaseLayer");
    }

    public Vector3 ProcessData(Vector3 hitpoint, Transform transpos)
    {
        Vector3 e = Quaternion.Inverse(transpos.rotation) * (hitpoint - transpos.position);
        if (Mathf.Abs(e.x) > Mathf.Abs(e.z))
        {
            return new Vector3(e.x >= 0 ? 1 : -1, 0, 0);
        }
        else
        {
            return new Vector3(0, 0, e.z >= 0 ? 1 : -1);
        }
    }

    public override Vector3 AutoAlign(Vector3 hitpoint, Transform hitobj, Transform target)
    {
        return hitobj.rotation * (Crafting.multiplyeach(ProcessData(hitpoint, hitobj), (hitobj.lossyScale + target.transform.lossyScale)) / 2);
    }
}
