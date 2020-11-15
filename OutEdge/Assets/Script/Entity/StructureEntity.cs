using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainManager;
using static ItemManager;
using Newtonsoft.Json;
using OutEdge;

[Serializable]
public class StorageStruct
{
    public List<StorageStruct> child = new List<StorageStruct>();
    public float posx;
    public float posy;
    public float posz;
    public float rotx;
    public float roty;
    public float rotz;
    public float rotw;
    public int id;

    public StorageStruct(Vector3 pos, Quaternion rot, int i)
    {
        posx = pos.x;
        posy = pos.y;
        posz = pos.z;
        rotx = rot.x;
        roty = rot.y;
        rotz = rot.z;
        rotw = rot.w;
        id = i;
    }
}

public class StructureEntity : EntityBase
{
    public StructureEntity parent;

    public virtual void Start()
    {
        if (transform.parent == null)
        {
            RefreshChunkImplement();
        }
        else
        {
            parent = transform.parent.GetComponent<StructureEntity>();
        }
    }

    public List<StorageStruct> findChild(Transform trans)
    {
        List<StorageStruct> ls = new List<StorageStruct>();
        foreach (Transform child in trans)
        {
            EntityBase eb = child.GetComponent<EntityBase>();
            if (eb != null)
            {
                StorageStruct storage = new StorageStruct(child.position, child.rotation, child.GetComponent<EntityBase>().id);
                storage.child = findChild(child);
                ls.Add(storage);
            }
        }
        return ls;
    }

    public string save()
    {
        return JsonConvert.SerializeObject(findChild(transform));
    }

    public virtual Vector3 AutoAlign(Vector3 hitpoint, Transform hitobj, Transform target)
    {
        return Vector3.zero;
    }

    public virtual Quaternion AutoRotate(Vector3 hitpoint,Transform hitobj,Transform target)
    {
        return hitobj.rotation;
    }

    public void load(string data)
    {
        List<StorageStruct> ls =  JsonConvert.DeserializeObject<List<StorageStruct>>(data);
        GenerateEntity(transform,ls);
    }

    public void GenerateEntity(Transform trans,List<StorageStruct> ls)
    {
        foreach(StorageStruct ss in ls)
        {
            GameObject nobj = Instantiate(EntityManager.em.entities[ss.id].prefab, new Vector3(ss.posx, ss.posy, ss.posz), new Quaternion(ss.rotx, ss.roty, ss.rotz, ss.rotw));
            nobj.transform.parent = trans;
            Destroy(nobj.GetComponent<Rigidbody>());
            Collider[] co = nobj.GetComponents<Collider>();
            foreach (Collider c in co)
            {
                c.isTrigger = false;
            }

            nobj.layer = 0;
            foreach (Transform child in nobj.transform)
            {
                child.gameObject.layer = 0;
            }
            try
            {
                nobj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
            catch { }
            try
            {
                nobj.GetComponent<EntityBase>().enabled = true;
            }
            catch { }
            GenerateEntity(nobj.transform, ss.child);
        }
    }

    public void OnDestroy()
    {
        if (lastChunk != null)
        {
            lastChunk.RemoveEntity(gameObject);
        }
    }

    public virtual void FixedUpdate()
    {
        if (transform.position.y < -5)
        {
            Destroy(gameObject);
        }
    }
}
