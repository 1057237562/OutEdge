using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainManager;

public class CenterObject : AttributeContainer
{
    public List<GameObject> objlist = new List<GameObject>();
    public int id;
    public bool pickable = true;

    public MarchingStack lastChunk;
    public Quaternion prerot = Quaternion.identity;

    public void RefreshChunkImplement()
    {
        if (lastChunk != null)
        {
            lastChunk.RemoveEntity(gameObject);
        }
        lastChunk = tm.GetChunk(tm.GetId(transform.position));
        lastChunk.AddEntity(gameObject);
    }

    private void Start()
    {
       
    }

    public void Regist()
    {
        GetComponent<CenterObject>().RefreshChunkImplement();
    }

    public void DeRegist()
    {
        if(lastChunk != null)
        {
            lastChunk.RemoveEntity(gameObject);
            lastChunk = null;
        }
    }

    public void Save()
    {
        //Pop player

        foreach (GameObject obj in objlist)
        {
            if (obj.tag == "Material")
            {
                obj.layer = LayerMask.NameToLayer("SnapLayer");
            }
            if (obj.GetComponent<Seat>() && obj.GetComponent<Seat>().enabled)
            {
                obj.GetComponent<Seat>().PopPlayer();
            }
        }

        FileSystem.SerializeGameObjects(objlist.ToArray(), gameObject, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(gameObject.GetComponent<CenterObject>().id) + ".oeg", new FileSystem.Content(Vector3.zero,GetComponent<CenterObject>().prerot), "Material");

        gameObject.AddComponent<SnapShot>();
        gameObject.GetComponent<SnapShot>().TakeSnapShot(gameObject, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(gameObject.GetComponent<CenterObject>().id) + ".oep", delegate{});

        foreach (GameObject obj in objlist)
        {
            if (obj.tag == "Material")
            {
                obj.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (lastChunk != null)
        {
            GetComponent<CenterObject>().RefreshChunkImplement();
        }
    }

    public static void CreateCenter(GameObject obj,bool pick){
        if (obj.GetComponent<CenterObject>() != null) return;
        obj.AddComponent<CenterObject>();
        Inventory.allocateItem.Add(obj.GetInstanceID());
        obj.GetComponent<CenterObject>().id = Inventory.GetCustomId(obj);
        obj.GetComponent<CenterObject>().Regist();
        List<GameObject> output = new List<GameObject>();
        PassDown(obj,obj,output,obj.GetComponent<ConnectiveMaterial>() != null ? obj.GetComponent<ConnectiveMaterial>().centerparent.GetComponent<CenterObject>().objlist : null);
        obj.GetComponent<CenterObject>().objlist = output;
        obj.GetComponent<CenterObject>().pickable = pick;
    }

    static void PassDown(GameObject center,GameObject obj,List<GameObject> output,List<GameObject> original = null){
        if (!obj) return;
        output.Add(obj);
        obj.GetComponent<ConnectiveMaterial>().centerparent = center;
        if(original != null)
            original.Remove(obj);
        List<GameObject> sons = obj.GetComponent<ConnectiveMaterial>().sons;
        foreach(GameObject son in sons){
            if(son)
                PassDown(center,son,output,original);
        }
    }
}
