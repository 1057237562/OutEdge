using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBase : StructureEntity
{
    private void OnEnable()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    public Vector3 ProcessData(Vector3 hitpoint, Transform transpos)
    {
        Vector3 e = Quaternion.Inverse(transpos.rotation) * (hitpoint - transpos.position);
        if (Mathf.Abs(e.x) > Mathf.Abs(e.z))
        {
            return new Vector3(e.x >= 0 ? 1 : -1, 1, 0);
        }
        else
        {
            return new Vector3(0, 1, e.z >= 0 ? 1 : -1);
        }
    }

    public Quaternion ProcessRot(Vector3 hitpoint, Transform transpos)
    {
        Vector3 e = Quaternion.Inverse(transpos.rotation) * (hitpoint - transpos.position);
        if (Mathf.Abs(e.x) > Mathf.Abs(e.z))
        {
            return Quaternion.Euler(0,90,0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public override Vector3 AutoAlign(Vector3 hitpoint, Transform hitobj, Transform target)
    {
        return hitobj.rotation * (Crafting.multiplyeach(ProcessData(hitpoint, hitobj), hitobj.lossyScale) / 2) + new Vector3(0, target.lossyScale.y/2 + 0.001f, 0);
    }

    public override Quaternion AutoRotate(Vector3 hitpoint, Transform hitobj, Transform target)
    {
        return hitobj.rotation*ProcessRot(hitpoint,hitobj);
    }

}
