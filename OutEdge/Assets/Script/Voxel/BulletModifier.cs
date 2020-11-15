//using BulletSharp;
//using BulletUnity;
//using BulletUnity.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletModifier : MonoBehaviour
{
    // Start is called before the first frame update
    /*public Mesh mesh;
    public bool func = false;
    private bool phy = false;

    BConvexHullShape bhs;
    BConvexHull bh;
    public BulletSharp.CollisionFlags cf;


    void Start()
    {
        bhs = gameObject.AddComponent<BConvexHullShape>();
        bh = gameObject.AddComponent<BConvexHull>();
        bh.meshSettings.UserMesh = mesh;
        bh.BuildMesh();
        GetComponent<BRigidBody>().collisionFlags = cf;
    }

    public void Update()
    {
        if (func)
        {
            ForceUpdateMesh();
            func = false;
            phy = true;
        }
    }

    public void FixedUpdate()
    {
        if (phy)
        {
            PostPhysics();
            phy = false;
        }
    }

    public void PostUpdatePhysics()
    {
        func = true;
    }

    public void ForceUpdateMesh()
    {
        if (bh != null)
        {
            Destroy(bh);
            //GetComponent<BRigidBody>().RemoveObjectFromBulletWorld();
            if (bhs.collisionShapePtr != null)
                bhs.collisionShapePtr.Dispose();
            Destroy(bhs);
            GetComponent<BRigidBody>().Dispose();
            GetComponent<BRigidBody>().enabled = false;
            Destroy(GetComponent<BRigidBody>());
            //bhs.HullMesh = mesh;
            //GetComponent<BRigidBody>().enabled = true;
            //GetComponent<BRigidBody>().AddObjectToBulletWorld();
        }
    }

    public void PostPhysics()
    {
        bhs = gameObject.AddComponent<BConvexHullShape>();
        bh = gameObject.AddComponent<BConvexHull>();
        bh.meshSettings.UserMesh = mesh;
        bh.BuildMesh();
        GetComponent<BRigidBody>().collisionFlags = cf;
    }*/
}
