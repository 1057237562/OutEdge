using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class BlockBase : StructureEntity
{
    public virtual void OnEnable()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    public Vector3 ProcessData(Vector3 hitpoint, Transform transpos)
    {
        Vector3 e = Quaternion.Inverse(transpos.rotation) * (hitpoint - transpos.position);
        if (Mathf.Abs(e.x) > Mathf.Abs(e.y) && Mathf.Abs(e.x) > Mathf.Abs(e.z))
        {
            return new Vector3(e.normalized.x, 0, 0).normalized;
        }
        if (Mathf.Abs(e.y) > Mathf.Abs(e.x) && Mathf.Abs(e.y) > Mathf.Abs(e.z))
        {
            return new Vector3(0, e.normalized.y, 0).normalized;
        }
        if (Mathf.Abs(e.z) > Mathf.Abs(e.x) && Mathf.Abs(e.z) > Mathf.Abs(e.y))
        {
            return new Vector3(0, 0, e.normalized.z).normalized;
        }
        return new Vector3(0, 0, 0);
    }

    public override Vector3 AutoAlign(Vector3 hitpoint, Transform hitobj, Transform target)
    {
        if (hitobj.GetComponent<BlockBase>() != null)
        {
            return hitobj.rotation * (Crafting.multiplyeach(ProcessData(hitpoint, hitobj), hitobj.lossyScale + target.lossyScale) / 2);
        }
        else
        {
            return -hitobj.transform.position + hitpoint + new Vector3(0, target.lossyScale.y / 2, 0);
        }
    }

    public override Quaternion AutoRotate(Vector3 hitpoint, Transform hitobj, Transform target)
    {
        if (hitobj.GetComponent<BlockBase>() != null)
        {
            return base.AutoRotate(hitpoint, hitobj, target);
        }
        else
        {
            return RigidbodyFirstPersonController.rfpc.transform.rotation;
        }
    }

}