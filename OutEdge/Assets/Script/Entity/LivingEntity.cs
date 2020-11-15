using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : EntityBase
{
    [Serializable]
    public class BodyPart
    {

        public Vector3 center;
        public Vector3 halfsize;

        public float DEATH_ACCELERATION = STANDARD_DEATH_ACCELERATION;

        public float HURT_ACCELERATION = STANDARD_HURT_ACCELERATION;

    }

    public const float STANDARD_DEATH_ACCELERATION = 1800;

    public const float STANDARD_HURT_ACCELERATION = 390;

    public int maxHealth;

    public float nowHealth;

    [SerializeField] public List<BodyPart> bodyParts;

    public void ForceAppendCollision(float acceleration, Vector3 hitPoint)
    {
        Vector3 localCP = transform.InverseTransformPoint(hitPoint);

        bool injured = false;

        foreach (BodyPart bp in bodyParts)
        {
            Vector3 direct = localCP - bp.center;

            if (Mathf.Abs(bp.halfsize.x) >= Mathf.Abs(direct.x) && Mathf.Abs(bp.halfsize.y) >= Mathf.Abs(direct.y) && Mathf.Abs(bp.halfsize.z) >= Mathf.Abs(direct.z))
            {
                if (acceleration >= bp.HURT_ACCELERATION)
                {
                    nowHealth -= (acceleration - bp.HURT_ACCELERATION) / (bp.DEATH_ACCELERATION - bp.HURT_ACCELERATION) * maxHealth;
                }
                injured = true;
                break;
            }
        }

        if (!injured)
        {
            //Debug.Log("acceleration:" + acceleration);
            if (acceleration >= STANDARD_HURT_ACCELERATION)
            {
                nowHealth -= (acceleration - STANDARD_HURT_ACCELERATION) / (STANDARD_DEATH_ACCELERATION - STANDARD_HURT_ACCELERATION) * maxHealth;
            }
        }
        if (nowHealth < 0)
        {
            Death();
        }
    }

    public Vector3 getFace(Vector3 pos, Collider cd, Vector3 extents)
    {
        Vector3 dis = cd.transform.rotation * (pos - cd.transform.position);
        Vector3 near = Crafting.absolute(Crafting.divideeach(Crafting.absolute(dis), extents) - new Vector3(1, 1, 1));
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

    public float ZeroToOne(float a)
    {
        return a == 0 ? 1 : a;
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

    void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (nowHealth < 0)
            {
                return;
            }
            ContactPoint cp = collision.GetContact(i);
            if (cp.otherCollider.GetComponent<MarchingObject>() != null)
            {
                if (collision.impulse.y / Time.fixedDeltaTime>25)
                {
                    nowHealth -= (collision.impulse.y / Time.fixedDeltaTime - 25) / 3;
                }
                continue;
            }
            if (cp.otherCollider.GetComponent<ItemBase>() != null || cp.otherCollider.GetComponent<StructureEntity>() != null)
            {
                continue;
            }
            float force;
            try
            {
                force = (collision.impulse / Time.fixedDeltaTime).magnitude * Mathf.Pow(cp.otherCollider.GetComponent<Rigidbody>().mass * 10, 2);
            }
            catch
            {
                continue;
            }
            float sharpness;
            float oarea;
            if (cp.otherCollider.GetComponent<MeshCollider>() != null)
            {
                //Debug.Log(i + ":" + name);
                Ray ray = new Ray(cp.point, -cp.normal);
                RaycastHit hitInfo;
                cp.otherCollider.Raycast(ray, out hitInfo, 1);
                Mesh mesh = ((MeshCollider)cp.otherCollider).sharedMesh;
                oarea = CalculateArea(mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3]], mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3 + 1]], mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3 + 2]]);
                sharpness = ComputeSharpness(mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3]], mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3 + 1]], mesh.vertices[mesh.triangles[hitInfo.triangleIndex * 3 + 2]]);
            }
            else
            {
                Vector3 other_extents = Crafting.multiplyeach(GetColliderSize(cp.otherCollider) / 2, cp.otherCollider.transform.lossyScale);
                Vector3 this_extents = Crafting.multiplyeach(GetColliderSize(cp.thisCollider) / 2, cp.thisCollider.transform.lossyScale);
                Vector3 otherface = Crafting.multiplyeach(other_extents, getFace(cp.point, cp.otherCollider, other_extents));

                Vector3 outfrompoint = cp.thisCollider.transform.InverseTransformPoint(cp.otherCollider.transform.TransformPoint(otherface));
                Vector3 outtopoint = cp.thisCollider.transform.InverseTransformPoint(cp.otherCollider.transform.TransformPoint(-otherface));

                Vector3 toextents = outfrompoint - outtopoint;

                Vector3 thisface = Crafting.PowEach(this_extents, getFace(cp.point, cp.thisCollider, this_extents));

                oarea = ZeroToOne(Mathf.Min(toextents.x, thisface.x)) * ZeroToOne(Mathf.Min(toextents.y, thisface.y)) * ZeroToOne(Mathf.Min(toextents.z, thisface.z));
                //Debug.Log(toextents + ":"+oarea + name);
                sharpness = Mathf.Max(otherface.x, Mathf.Max(otherface.y, otherface.z)) / Mathf.Min(otherface.x != 0 ? otherface.x : Mathf.Infinity, Mathf.Min(otherface.y != 0 ? otherface.y : Mathf.Infinity, otherface.z != 0 ? otherface.z : Mathf.Infinity));
            }
            ForceAppendCollision(force * Mathf.Sqrt(sharpness) / Mathf.Abs(oarea) * 10, collision.GetContact(i).point);
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

        return side[1]/side[0];
    }
}
