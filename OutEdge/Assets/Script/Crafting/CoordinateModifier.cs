using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemManager;

public class CoordinateModifier : MonoBehaviour
{
    public GameObject desk;
    public GameObject selected;

    public GameObject x;
    public GameObject y;
    public GameObject z;

    public GameObject xd;
    public GameObject yd;
    public GameObject zd;

    public float startdistance;

    public Slider duration;

    public GameObject op;
    //public GameObject centerObject;

    public int type;

    GameObject hit;
    Vector3 hitpoint;

    Vector3 lastpoint = Vector3.zero;

    Vector3 startpoint = Vector3.zero;
    Vector3 rip_euler = new Vector3(0, 0, 0);

    public float mousespeed;

    List<Rigidbody> o_connected;

    public Vector3 relative = Vector3.zero;

    Material tempm;

    public Material selectMaterial;

    public void SetDesk(GameObject desk)
    {
        this.desk = desk;
    }

    public void Disable()
    {
        transform.parent = null;
        if (selected != null)
        {
            if (selected.layer != 5)
            {
                try
                {
                    selected.GetComponent<Collider>().enabled = true;
                    selected.GetComponent<Rigidbody>().isKinematic = false;
                    foreach (Transform child in selected.transform)
                    {
                        //child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                        child.GetComponent<Collider>().enabled = true;
                        child.GetComponent<Rigidbody>().isKinematic = false;
                    }
                }
                catch { }
                if (Crafting.autoLock)
                {
                    if (Crafting.m.locklast != selected)
                    {
                        Crafting.m.locklast.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                        Crafting.m.locklast = selected;
                        selected.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                }
                else
                {
                    if (Crafting.m.locklast)
                    {
                        Crafting.m.locklast.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                    else
                    {
                        Crafting.m.locklast = selected.GetComponent<ConnectiveMaterial>().centerparent;
                        Crafting.m.locklast.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                }

                Joint[] joints = selected.GetComponents<Joint>();

                for (int i = 0; i < o_connected.Count; i++)
                {
                    joints[i].connectedBody = o_connected[i];
                    o_connected[i].constraints = RigidbodyConstraints.None;
                    joints[i].breakForce = selected.GetComponent<ConnectiveMaterial>().breakForce;
                    joints[i].breakTorque = selected.GetComponent<ConnectiveMaterial>().breakTorque;
                }
                selected.GetComponent<ConnectiveMaterial>().LinkTarget();
            }
        }

        o_connected = null;

        op.SetActive(false);
        selected = null;
        desk = null;
    }

    public void Enable()
    {
        op.SetActive(true);

        if (selected.layer != 5)
        {
            if (Crafting.m.locklast)
                Crafting.m.locklast.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            try
            {
                selected.GetComponent<Rigidbody>().isKinematic = true;
                selected.GetComponent<Collider>().enabled = false;
                foreach (Transform child in selected.transform)
                {
                    //child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    child.GetComponent<Collider>().enabled = false;
                    child.GetComponent<Rigidbody>().isKinematic = true;
                }
                foreach (Joint j in selected.GetComponents<Joint>())
                {
                    j.connectedBody.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
            catch { }

            o_connected = new List<Rigidbody>();

            foreach (Joint joint in selected.GetComponents<Joint>())
            {
                if (joint.connectedBody)
                {
                    o_connected.Add(joint.connectedBody);
                    joint.connectedBody = null;
                    joint.breakForce = Mathf.Infinity;
                    joint.breakTorque = Mathf.Infinity;
                }
                else
                {
                    Destroy(joint);
                }
            }
        }
        if (type == 2)
        {
            startpoint = new Vector3(startdistance, startdistance, startdistance);
        }
    }

    Vector3 dis;
    Vector3 point;

    void Update()
    {
        if (hit != null)
        {
            UpdateContent();
            /*if (hit == x || hit == y || hit == z)
            {
                Crafting.block = true;
            }*/
            Camera camera = desk.GetComponent<Crafter>().tc;
            switch (type)
            {
                case 0:
                    Vector3 css = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis.z);
                    Vector3 cp = camera.ScreenToWorldPoint(css) + point;
                    if (lastpoint == Vector3.zero)
                    {
                        lastpoint = cp;
                    }
                    //float tempLength = Vector3.Distance(currentPosition, transform.position);
                    Vector3 tp = Vector3.zero;
                    if (hit == x)
                    {
                        tp = Vector3.Project(cp - lastpoint, transform.forward);
                    }
                    if (hit == y)
                    {
                        tp = Vector3.Project(cp - lastpoint, transform.up);
                    }
                    if (hit == z)
                    {
                        tp = Vector3.Project(cp - lastpoint, transform.right);
                    }
                    transform.position += tp;
                    lastpoint = cp;
                    break;
                case 1:
                    Vector3 cus = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis.z);
                    Vector3 curpo = camera.ScreenToWorldPoint(cus);
                    Vector3 hp = GetIntersectWithLineAndPlane(camera.transform.position, (curpo - camera.transform.position).normalized, hit.transform.forward, hit.transform.position);
                    if (lastpoint == Vector3.zero)
                    {
                        lastpoint = hp;
                    }

                    Quaternion result = Quaternion.FromToRotation(hit.transform.InverseTransformPoint(lastpoint), hit.transform.InverseTransformPoint(hp));

                    if (hit == x)
                    {
                        if(Crafting.m.global) transform.rotation *= Quaternion.Euler(result.eulerAngles.z > 180 ? result.eulerAngles.z - 360 : result.eulerAngles.z, 0, 0);
                        rip_euler += new Vector3(result.eulerAngles.z > 180 ? result.eulerAngles.z - 360: result.eulerAngles.z, 0,0);
                    }
                    if (hit == y)
                    {
                        if (Crafting.m.global) transform.rotation *= Quaternion.Euler(0, result.eulerAngles.z > 180 ? 360 - result.eulerAngles.z : -result.eulerAngles.z, 0);
                        rip_euler += new Vector3(0, result.eulerAngles.z > 180 ? 360 - result.eulerAngles.z : -result.eulerAngles.z, 0);
                    }
                    if (hit == z)
                    {
                       if (Crafting.m.global) transform.rotation *= Quaternion.Euler(0, 0, result.eulerAngles.z > 180 ? 360 - result.eulerAngles.z : -result.eulerAngles.z);
                        rip_euler += new Vector3(0, 0, result.eulerAngles.z > 180 ? 360 - result.eulerAngles.z : -result.eulerAngles.z);
                    }
                    
                    lastpoint = hp;
                    break;
                case 2:
                    Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis.z);
                    Vector3 currentPosition = camera.ScreenToWorldPoint(currentScreenSpace) + point;
                    if(lastpoint == Vector3.zero)
                    {
                        lastpoint = currentPosition;
                    }
                    //float tempLength = Vector3.Distance(currentPosition, transform.position);
                    Vector3 tempPos = Vector3.zero;
                    if (hit == x)
                    {
                        tempPos = Vector3.Project(currentPosition - lastpoint, transform.forward);
                    }
                    if (hit == y)
                    {
                        tempPos = Vector3.Project(currentPosition - lastpoint, transform.up);
                    }
                    if (hit == z)
                    {
                        tempPos = Vector3.Project(currentPosition - lastpoint, transform.right);
                    }
                    hit.transform.position += tempPos;
                    lastpoint = currentPosition;
                    break;
            }
        }
        if (Input.GetMouseButtonDown(0) && desk != null && selected != null)
        {
            Ray ray = desk.GetComponent<Crafter>().tc.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << (LayerMask.NameToLayer("UILayer"))))
            {
                hit = hitInfo.collider.gameObject;
                tempm = hit.GetComponent<MeshRenderer>().material;
                hit.GetComponent<MeshRenderer>().material = selectMaterial;
                hitpoint = hitInfo.point;
                relative = transform.InverseTransformPoint(hitpoint);
                Camera camera = desk.GetComponent<Crafter>().tc;
                dis = camera.WorldToScreenPoint(transform.position);
                if (type == 2)
                {
                    point = hit.transform.position - camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, dis.z));
                }
            }
        }
        if (desk != null && selected != null && !Input.GetMouseButton(0))
        {
            if(hit)hit.GetComponent<MeshRenderer>().material = tempm;
            hit = null;
            lastpoint = Vector2.zero;
            if (selected != null)
            {
                transform.position = selected.transform.position;
                switch (type)
                {
                    case 1:
                        startpoint = Vector3.zero;
                        transform.localRotation = Quaternion.identity;
                        rip_euler = new Vector3(0, 0, 0);
                        break;
                    case 2:
                        startpoint = new Vector3(startdistance, startdistance, startdistance);
                        x.transform.localPosition = new Vector3(0.5f, 0.3f, startdistance);
                        y.transform.localPosition = new Vector3(0.3f, startdistance, -5.4f);
                        z.transform.localPosition = new Vector3(startdistance, -0.3f, -4.5f);
                        zd.transform.localScale = new Vector3(zd.transform.localScale.x, zd.transform.localScale.y, 0.6f);
                        xd.transform.localScale = new Vector3(xd.transform.localScale.x, xd.transform.localScale.y, 0.6f);
                        yd.transform.localScale = new Vector3(yd.transform.localScale.x, yd.transform.localScale.y, 0.6f);
                        break;
                }
            }
        }
    }

    private Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
    {
        float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
        //print(d);
        return d * direct.normalized + point;
    }

    void UpdateContent()
    {
        switch (type)
        {
            case 0:
                Vector3 movement = Quaternion.Inverse(transform.rotation)*(transform.position - selected.transform.position);
                if (Mathf.Abs(movement.x) >= duration.value)
                {
                    selected.transform.Translate(new Vector3((int)(movement.x / duration.value) * duration.value, 0, 0), Crafting.m.global ? Space.World : Space.Self);
                }
                if (Mathf.Abs(movement.y) >= duration.value)
                {
                    selected.transform.Translate(new Vector3(0, (int)(movement.y / duration.value) * duration.value, 0), Crafting.m.global ? Space.World : Space.Self);
                }
                if (Mathf.Abs(movement.z) >= duration.value)
                {
                    selected.transform.Translate(new Vector3(0, 0, (int)(movement.z / duration.value) * duration.value), Crafting.m.global ? Space.World : Space.Self);
                }
                break;
            case 1:
                if (Math.Abs(rip_euler.x - startpoint.x) >= duration.value)
                {
                    selected.transform.Rotate(new Vector3((int)((rip_euler.x - startpoint.x) / duration.value) * duration.value, 0, 0), Crafting.m.global ? Space.World:Space.Self);
                    startpoint += new Vector3((int)((rip_euler.x - startpoint.x) / duration.value) * duration.value, 0, 0);
                }
                if (Math.Abs(rip_euler.y - startpoint.y) >= duration.value)
                {
                    selected.transform.Rotate(new Vector3(0, (int)((rip_euler.y - startpoint.y) / duration.value) * duration.value, 0), Crafting.m.global ? Space.World : Space.Self);
                    startpoint += new Vector3(0, (int)((rip_euler.y - startpoint.y) / duration.value) * duration.value, 0);
                }
                if (Math.Abs(rip_euler.z - startpoint.z) >= duration.value)
                {
                    selected.transform.Rotate(new Vector3(0, 0, (int)((rip_euler.z - startpoint.z) / duration.value) * duration.value), Crafting.m.global ? Space.World : Space.Self);
                    startpoint += new Vector3(0, 0, (int)((rip_euler.z - startpoint.z) / duration.value) * duration.value);
                }
                break;
            case 2:
                Vector3 lastscale = selected.transform.localScale;
                if (Mathf.Abs(z.transform.localPosition.x - startpoint.x) >= duration.value)
                {
                    selected.transform.localScale += new Vector3((int)((z.transform.localPosition.x - startpoint.x) / duration.value) * duration.value / 100f, 0, 0);
                    zd.transform.localScale = new Vector3(zd.transform.localScale.x, zd.transform.localScale.y, z.transform.localPosition.x*6/500);
                    startpoint.x = z.transform.localPosition.x;
                }
                if (Mathf.Abs(y.transform.localPosition.y - startpoint.y) >= duration.value)
                {
                    selected.transform.localScale += new Vector3(0, (int)((y.transform.localPosition.y - startpoint.y) / duration.value) * duration.value/100f, 0);
                    yd.transform.localScale = new Vector3(yd.transform.localScale.x, yd.transform.localScale.y, y.transform.localPosition.y*6/500);
                    startpoint.y = y.transform.localPosition.y;
                }
                if (Mathf.Abs(x.transform.localPosition.z - startpoint.z) >= duration.value)
                {
                    selected.transform.localScale += new Vector3(0, 0, (int)((x.transform.localPosition.z - startpoint.z) / duration.value) * duration.value / 100f);
                    xd.transform.localScale = new Vector3(xd.transform.localScale.x, xd.transform.localScale.y, x.transform.localPosition.z*6/500);
                    startpoint.z = x.transform.localPosition.z;
                }

                if (selected.transform.localScale.x > Crafting.m.scaleLimit)
                {
                    selected.transform.localScale = new Vector3(Crafting.m.scaleLimit, selected.transform.localScale.y, selected.transform.localScale.z);
                }
                if (selected.transform.localScale.x < 0.01f)
                {
                    selected.transform.localScale = new Vector3(0.02f, selected.transform.localScale.y, selected.transform.localScale.z);
                }
                if (selected.transform.localScale.y > Crafting.m.scaleLimit)
                {
                    selected.transform.localScale = new Vector3(selected.transform.localScale.x, Crafting.m.scaleLimit, selected.transform.localScale.z);
                }
                if (selected.transform.localScale.y < 0.01f)
                {
                    selected.transform.localScale = new Vector3(selected.transform.localScale.x, 0.02f, selected.transform.localScale.z);
                }
                if (selected.transform.localScale.z > Crafting.m.scaleLimit)
                {
                    selected.transform.localScale = new Vector3(selected.transform.localScale.x, selected.transform.localScale.y, Crafting.m.scaleLimit);
                }
                if (selected.transform.localScale.z < 0.01f)
                {
                    selected.transform.localScale = new Vector3(selected.transform.localScale.x, selected.transform.localScale.y, 0.02f);
                }

                float multiplier = selected.transform.localScale.x * selected.transform.localScale.y * selected.transform.localScale.z - lastscale.x * lastscale.y * lastscale.z;

                int id;
                try
                {
                    Crafting.reflect.TryGetValue(selected.name.Replace("(Clone)", ""), out id);
                    id--;
                    if (multiplier > 0)
                    {
                        if (!Inventory.m.removeItems(Crafting.m.materialCosts[id].material, multiplier))
                        {
                            selected.transform.localScale = lastscale;
                            return;
                        }
                    }
                    else if(multiplier < 0)
                    {
                        Inventory.m.attemptAddItems(Crafting.m.materialCosts[id].material, -multiplier);
                    }
                }
                catch
                {
                    return;
                }

                selected.GetComponent<Rigidbody>().mass *= multiplier;
                selected.GetComponent<ConnectiveMaterial>().RefreshJoint();
                selected.GetComponent<Rigidbody>().isKinematic = true;

                SimpleWing sw = selected.GetComponent<SimpleWing>();
                if (sw != null)
                {
                    sw.dimensions = new Vector2(selected.transform.localScale.x, selected.transform.localScale.z);
                    sw.liftMultiplier = selected.transform.localScale.x * selected.transform.localScale.y * selected.transform.localScale.z;
                }
                 // Temporary
                break;
        }
    }
}
