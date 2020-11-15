using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilBase : StructureEntity
{
    private void OnEnable()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    public Vector3 ProcessData(Vector3 hitpoint, Transform transpos)
    {
        Vector3 e = Quaternion.Inverse(transpos.rotation) * (hitpoint - transpos.position);
        if (Mathf.Abs(e.x)* transpos.lossyScale.z > Mathf.Abs(e.z) * transpos.lossyScale.x)
        {
            return new Vector3(e.x >= 0 ? 1 : -1, 1, 0);
        }
        else
        {
            return new Vector3(0, 1, e.z >= 0 ? 1 : -1);
        }
    }

    public Vector3 PillarAlign(Vector3 hitpoint, Transform transpos)
    {
        Vector3 e = Quaternion.Inverse(transpos.rotation) * (hitpoint - transpos.position);
        return new Vector3(e.x >= 0 ? 1 : -1, 1, e.z >= 0 ? 1 : -1);
    }

    public override Vector3 AutoAlign(Vector3 hitpoint, Transform hitobj, Transform target)
    {
        if (hitobj.GetComponent<PillarBase>() != null)
        {
            return hitobj.rotation * (Crafting.multiplyeach(PillarAlign(hitpoint, hitobj), new Vector3(0,hitobj.lossyScale.y,0) + target.lossyScale) / 2) + new Vector3(0,0.01f,0);
        }
        else
        {
            FloorBase cb = hitobj.GetComponent<FloorBase>();
            return hitobj.rotation * (Crafting.multiplyeach(ProcessData(hitpoint, hitobj), target.lossyScale + new Vector3(cb != null ? hitobj.lossyScale.x : 0, cb != null ? -target.lossyScale.y : hitobj.lossyScale.y, cb != null ? hitobj.lossyScale.z : 0)) / 2) + new Vector3(0, cb == null ? 0.01f : 0, 0);
        }
    }
}
