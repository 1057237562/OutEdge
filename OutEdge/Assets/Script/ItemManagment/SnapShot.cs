using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SnapShot : MonoBehaviour
{
    public GameObject tar;
    private string n;
    //private GameObject[] objects;
    private Action dodone;

    public static RenderTexture renderTexture;

    public static float gasp = 2f;

    public void TakeSnapShot(GameObject center,string filename,Action action)
    {
        tar = new GameObject();
        tar.AddComponent<Camera>();

        Vector3 maxpoint = new Vector3(0,0,0);

        List<GameObject> objlist = center.GetComponent<CenterObject>().objlist;

        foreach(GameObject obj in objlist){
            Vector3 v = center.transform.InverseTransformPoint(obj.transform.position);
            if(v.x > maxpoint.x){
                maxpoint.x = v.x;
            }
            if(v.y > maxpoint.y){
                maxpoint.y = v.y;
            }
            if(v.z > maxpoint.z){
                maxpoint.z = v.z;
            }
        }

        tar.transform.position = center.transform.TransformPoint(maxpoint + new Vector3(gasp,gasp,gasp));
        tar.transform.LookAt(center.transform);
        tar.GetComponent<Camera>().cullingMask = 1<<LayerMask.NameToLayer("SnapLayer");
        tar.GetComponent<Camera>().targetTexture = renderTexture;

        dodone = action;
        //objects = c;
        n = filename;
        if (!Directory.Exists(filename.Substring(0, filename.LastIndexOf("/"))))
        {
            Directory.CreateDirectory(filename.Substring(0, filename.LastIndexOf("/")));
        }
    }

    void FixedUpdate()
    {
        if (tar != null)
        {
            Camera @cam = tar.GetComponent<Camera>();

            RenderTexture @render = cam.targetTexture;
            RenderTexture.active = render;

            Texture2D @texture = new Texture2D(render.width, render.height);
            texture.ReadPixels(new Rect(0, 0, render.width, render.height), 0, 0);
            texture.Apply();

            Debug.Log("Snapshot Saved to:"+n);

            byte[] @vs = texture.EncodeToPNG();
            FileStream file = File.Create(n);
            BinaryWriter binary = new BinaryWriter(file);
            binary.Write(vs);
            file.Close();

            Destroy(tar);

            RenderTexture.active = null;
            dodone();
        }
    }
}
