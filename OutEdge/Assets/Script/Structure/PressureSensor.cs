using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSensor : BlockBase
{
    public Queue<string> data = new Queue<string>(3);
    public GameObject hpspite;
    public bool locked = false;

    public override void OnEnable()
    {
    }

    public override void FixedUpdate()
    {
        locked = false;
        base.FixedUpdate();
    }

    public override void Start()
    {
        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        base.Start();
    }

    public TextMesh tm;
    public void ForceAppendCollision(float acceleration, Vector3 hitPoint)
    {
        if (!locked && acceleration > 0)
        {
            if (data.Count == 3)
            {
                data.Dequeue();
            }
            data.Enqueue(acceleration + " Pa");
            string stringbuilder = "";
            foreach (string s in data)
            {
                stringbuilder += s + "\n";
            }
            tm.text = stringbuilder;
            locked = true;
        }
        
    }

    public Vector3 getFace(Vector3 pos, Collider cd, Vector3 extents)
    {
        Vector3 dis = cd.transform.rotation * (pos - cd.transform.position);
        Vector3 near = Crafting.absolute(Crafting.divideeach(Crafting.absolute(dis) ,extents)-new Vector3(1,1,1));
        if (near.x < near.y)
        {
            if (near.x < near.z)
            {
                return new Vector3(0, 1, 1);
            }
            else
            {
                return new Vector3(1, 1, 0);
            }
        }
        else
        {
            if (near.y < near.z)
            {
                return new Vector3(1, 0, 1);
            }
            else
            {
                return new Vector3(1, 1, 0);
            }
        }
    }

    public Vector3 GetColliderSize(Collider collider)
    {
        switch (collider.GetType().Name)
        {
            case "BoxCollider":
                return ((BoxCollider)collider).size;
            case "CapsuleCollider":
                return new Vector3(((CapsuleCollider)collider).radius * 2, ((CapsuleCollider)collider).height, ((CapsuleCollider)collider).radius * 2);
            case "SphereCollider":
                return new Vector3(((SphereCollider)collider).radius * 2, ((SphereCollider)collider).radius * 2, ((SphereCollider)collider).radius * 2);
        }
        return Vector3.zero;
    }

    public float ZeroToOne(float a)
    {
        return a == 0 ? 1 : a;
    }

    void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint cp = collision.GetContact(i);
            if (cp.otherCollider.GetComponent<ItemBase>() != null || cp.otherCollider.GetComponent<StructureEntity>() != null)
            {
                continue;
            }
            float force;
            try
            {
                Debug.Log(collision.impulse);
                force = (collision.impulse / Time.fixedDeltaTime).magnitude * Mathf.Pow(cp.otherCollider.GetComponent<Rigidbody>().mass*10,2);
            }
            catch
            {
                return;
            }
            float sharpness;
            float oarea;
            if (cp.otherCollider.GetComponent<MeshCollider>() != null)
            {
                if (cp.otherCollider.GetComponent<MarchingObject>() != null)
                {

                    return;
                }
                else
                {
                    //Debug.Log(i + ":" + name);
                    Ray ray = new Ray(cp.point, -cp.normal);
                    RaycastHit hitInfo;
                    cp.otherCollider.Raycast(ray, out hitInfo, 1);
                    Mesh mesh = ((MeshCollider)cp.otherCollider).sharedMesh;
                    oarea = CalculateArea(mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3]], mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3 + 1]], mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3 + 2]]);
                    sharpness = ComputeSharpness(mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3]], mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3 + 1]], mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3 + 2]]);
                }
            }
            else
            {
                Vector3 other_extents = Crafting.multiplyeach(GetColliderSize(cp.otherCollider) / 2, cp.otherCollider.transform.lossyScale);
                Vector3 this_extents = Crafting.multiplyeach(GetColliderSize(cp.thisCollider) / 2, cp.thisCollider.transform.lossyScale);
                Vector3 otherface = Crafting.multiplyeach(other_extents, getFace(cp.point, cp.otherCollider, other_extents));

                Vector3 outfrompoint = cp.thisCollider.transform.InverseTransformPoint(cp.otherCollider.transform.TransformPoint(otherface));
                Instantiate(hpspite, cp.otherCollider.transform.TransformPoint(otherface),Quaternion.identity).SetActive(true);
                Vector3 outtopoint = cp.thisCollider.transform.InverseTransformPoint(cp.otherCollider.transform.TransformPoint(-otherface));
                Instantiate(hpspite, cp.otherCollider.transform.TransformPoint(-otherface), Quaternion.identity).SetActive(true);

                Vector3 toextents = outfrompoint - outtopoint;

                Vector3 thisface = Crafting.PowEach(this_extents, getFace(cp.point, cp.thisCollider, this_extents));

                oarea = ZeroToOne(Mathf.Min(toextents.x, thisface.x)) * ZeroToOne(Mathf.Min(toextents.y, thisface.y)) * ZeroToOne(Mathf.Min(toextents.z, thisface.z));
                //Debug.Log(toextents + ":"+oarea + name);
                sharpness = Mathf.Max(otherface.x, Mathf.Max(otherface.y, otherface.z)) / Mathf.Min(otherface.x != 0 ? otherface.x : Mathf.Infinity, Mathf.Min(otherface.y != 0 ? otherface.y : Mathf.Infinity, otherface.z != 0 ? otherface.z : Mathf.Infinity));
            }
            ForceAppendCollision(force * Mathf.Sqrt(sharpness) / oarea*10, collision.GetContact(i).point);
        }
    }

    private float CalculateArea(Vector3 pt0, Vector3 pt1, Vector3 pt2)
    {
        pt0 = new Vector3(pt0.x, pt0.y, pt0.z);
        pt1 = new Vector3(pt1.x, pt1.y, pt1.z);
        pt2 = new Vector3(pt2.x, pt2.y, pt2.z);

        float a = (pt1 - pt0).magnitude;
        float b = (pt2 - pt1).magnitude;
        float c = (pt0 - pt2).magnitude;
        float p = (a + b + c) * 0.5f;

        return Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
    }

    private float ComputeSharpness(Vector3 pt0, Vector3 pt1, Vector3 pt2)
    {
        pt0 = new Vector3(pt0.x, pt0.y, pt0.z);
        pt1 = new Vector3(pt1.x, pt1.y, pt1.z);
        pt2 = new Vector3(pt2.x, pt2.y, pt2.z);

        List<float> side = new List<float>();
        side.Add((pt1 - pt0).magnitude);
        side.Add((pt2 - pt1).magnitude);
        side.Add((pt0 - pt2).magnitude);
        side.Sort();

        return side[1] / side[0];
    }
}
