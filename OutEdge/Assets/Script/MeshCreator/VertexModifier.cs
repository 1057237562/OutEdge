using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemManager;

public class VertexModifier : MonoBehaviour
{
    public GameObject mo;
    public GameObject visual;

    public GameObject x;
    public GameObject y;
    public GameObject z;

    public Camera cam;

    public float startdistance;
    public int index = 0;
    public int ti = 0;
    public bool direct = false;
    //public GameObject centerObject;WS

    GameObject hit;

    Vector3 lastpoint = Vector3.zero;

    public float mousespeed;

    Vector3 dis;
    Vector3 point;

    private void OnEnable()
    {
        cam.GetComponent<CreatorCamera>().enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {

            MeshObject mesh = mo.GetComponent<MeshObject>();
            mesh.ModifyPoint(direct ? index:mesh.trianglesco[ti * 3 + index], mo.transform.InverseTransformPoint(visual.transform.position));

            cam.GetComponent<CreatorCamera>().enabled = true;
            direct = false;
            gameObject.SetActive(false);
        }
        if (hit != null)
        {
            UpdateContent();
            dis = cam.WorldToScreenPoint(transform.position);
            point = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis.z)) * mousespeed;
            if (hit == x)
            {
                if (lastpoint == Vector3.zero)
                {
                    lastpoint = point;
                }
                transform.Translate(new Vector3(0, 0, point.z - lastpoint.z), Space.Self);
                lastpoint = point;
            }
            if (hit == y)
            {
                if (lastpoint == Vector3.zero)
                {
                    lastpoint = point;
                }
                transform.Translate(new Vector3(0, point.y - lastpoint.y, 0), Space.Self);
                lastpoint = point;
            }
            if (hit == z)
            {
                if (lastpoint == Vector3.zero)
                {
                    lastpoint = point;
                }
                transform.Translate(new Vector3(point.x - lastpoint.x, 0, 0), Space.Self);
                lastpoint = point;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << (LayerMask.NameToLayer("UILayer"))))
            {
                hit = hitInfo.collider.gameObject;
            }
        }
        if (!Input.GetMouseButton(0))
        {
            hit = null;
            lastpoint = Vector2.zero;
        }
    }

    void UpdateContent()
    {
        visual.transform.position = transform.position;
    }
}
