using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using System;
using System.IO.Compression;
using UnityEditor;

public class FileSystem:MonoBehaviour
{

    /*public class Ternary
    {
        public float x = 0f;
        public float y = 0f;
        public float z = 0f;

        public Ternary(float dx, float dy, float dz)
        {
            x = dx;
            y = dy;
            z = dz;
        }

        public static Ternary FromVector(Vector3 origin)
        {
            return new Ternary(origin.x, origin.y, origin.z);
        }

        public string ToString()
        {
            return (x + ":" + y + ":" + z);
        }

    }*/

    public struct Content{
        public Vector3 position;
        public Quaternion rotation;

        public Content(Vector3 p,Quaternion r){
            position = p;
            rotation = r;
        }
    }

    public static Content RotateAround(GameObject obj,Transform center){
        Quaternion rotation;
        Vector3 position;
        if (obj.transform != center)
        {
            obj.transform.parent = center;
            rotation = obj.transform.localRotation;
            position = obj.transform.localPosition;
            obj.transform.parent = null;
        }
        else
        {
            rotation = Quaternion.identity;
            position = Vector3.zero;
        }
        //Debug.LogWarning(Quaternion.Inverse(rotate).eulerAngles + ":" +(obj.transform.rotation * rotate).eulerAngles);
        return new Content(position,rotation);
    }

    public static void SerializeGameObjects(GameObject[] objects,GameObject center, string filename,Content shift = new Content(),string tag = null)
    {
        if (!Directory.Exists(filename.Substring(0,filename.LastIndexOf("/"))))
        {
            Directory.CreateDirectory(filename.Substring(0, filename.LastIndexOf("/")));
        }

        XmlDocument content = new XmlDocument();
        XmlDeclaration dec = content.CreateXmlDeclaration("1.0", "UTF-8", null);
        content.AppendChild(dec);

        XmlElement objs = content.CreateElement("Objects");
        foreach (GameObject obj in objects)
        {
            if(obj.CompareTag(tag) || tag == null)
            {
                XmlElement xml = SerializeGameObject(obj,center.transform, content,shift);
                if(obj == center)
                    xml.SetAttribute("ID", center.GetComponent<CenterObject>().id+"");
                if (xml != null)
                    objs.AppendChild(xml);
            }
        }

        /*XmlElement ig = content.CreateElement("ItemGroup");

        ig.SetAttribute("ID",center.GetComponent<CenterObject>().id);

        XmlElement pos = content.CreateElement("Position");
        Vector3 position = center.transform.localPosition;
        pos.SetAttribute("X", position.x.ToString());
        pos.SetAttribute("Y", position.y.ToString());
        pos.SetAttribute("Z", position.z.ToString());

        ig.AppendChild(pos);

        XmlElement scale = content.CreateElement("Scale");
        scale.SetAttribute("X", itemgroup.transform.localScale.x.ToString());
        scale.SetAttribute("Y", itemgroup.transform.localScale.y.ToString());
        scale.SetAttribute("Z", itemgroup.transform.localScale.z.ToString());

        ig.AppendChild(scale);

        objs.AppendChild(ig);
        */
        content.AppendChild(objs);

        Debug.LogWarning("Saved to:"+filename);
        content.Save(filename);
    }

    public static XmlElement SerializeGameObject(GameObject obj,Transform center,XmlDocument doc,Content shift,bool parented = false,GameObject parent = null)
    {
        XmlElement node;

        int id;

        Transform collider = obj.transform;

        if (obj.transform.parent != null)
        {
            return null;
        }
        else
        {
            Crafting.reflect.TryGetValue(obj.gameObject.name.Replace("(Clone)", ""), out id);
        }

        if (obj.gameObject.transform == center)
        {
            node = doc.CreateElement("CenterObject");
        }
        else if(parented)
        {
            node = doc.CreateElement("ChildObject");
        }
        else
        {
            node = doc.CreateElement("Object");
        }

        node.SetAttribute("id",id.ToString());
        node.SetAttribute("uid", collider.gameObject.GetInstanceID().ToString());

        XmlElement transform = doc.CreateElement("Transform");
        XmlElement pos = doc.CreateElement("Position");

        Content post;
        if (parented)
        {
            obj.transform.parent = parent.transform;
            post = new Content(obj.transform.localPosition, obj.transform.localRotation);
        }
        else
        {
            if (obj.gameObject.transform == center)
            {
                post = shift;
            }
            else
            {
                post = RotateAround(obj, center);
            }
        }
        
        //Restore
        /*ConnectiveMaterial cm = obj.GetComponent<ConnectiveMaterial>();
        if (cm != null)
        {
            cm.Restore();
        }*/

        Vector3 position = post.position;
        pos.SetAttribute("X", position.x.ToString());
        pos.SetAttribute("Y", position.y.ToString());
        pos.SetAttribute("Z", position.z.ToString());

        transform.AppendChild(pos);

        XmlElement rotate = doc.CreateElement("Rotation");
        rotate.SetAttribute("X", post.rotation.x.ToString());
        rotate.SetAttribute("Y", post.rotation.y.ToString());
        rotate.SetAttribute("Z", post.rotation.z.ToString());
        rotate.SetAttribute("W", post.rotation.w.ToString());

        transform.AppendChild(rotate);

        XmlElement scale = doc.CreateElement("Scale");
        scale.SetAttribute("X", collider.localScale.x.ToString());
        scale.SetAttribute("Y", collider.localScale.y.ToString());
        scale.SetAttribute("Z", collider.localScale.z.ToString());

        transform.AppendChild(scale);

        HingeJoint hj = obj.GetComponent<HingeJoint>();
        if (hj)
        {
            transform.SetAttribute("CurrentPos", hj.spring.targetPosition.ToString());
        }

        Joint[] connection = obj.gameObject.GetComponents<Joint>();
        foreach (Joint joint in connection)
        {
            if (!joint.connectedBody || joint.connectedBody.gameObject == parent)
            {
                continue;
            }
            XmlElement connect = doc.CreateElement(parented ? "ChildConnection" : "Connection");

            try
            {
                ConfigurableJoint cj = (ConfigurableJoint)joint;
                if (cj.targetPosition != Vector3.zero)
                {
                    XmlElement anchor = doc.CreateElement("Anchor");
                    anchor.SetAttribute("X", cj.targetPosition.x.ToString());
                    anchor.SetAttribute("Y", cj.targetPosition.y.ToString());
                    anchor.SetAttribute("Z", cj.targetPosition.z.ToString());
                    connect.AppendChild(anchor);

                    anchor = doc.CreateElement("AnchorRotation");
                    anchor.SetAttribute("X", cj.targetRotation.x.ToString());
                    anchor.SetAttribute("Y", cj.targetRotation.y.ToString());
                    anchor.SetAttribute("Z", cj.targetRotation.z.ToString());
                    anchor.SetAttribute("W", cj.targetRotation.w.ToString());
                    connect.AppendChild(anchor);
                }
            }
            catch { }

            if (joint.connectedBody.transform.parent != null)
            {
                int c;
                for (c = 0; c < joint.connectedBody.transform.parent.childCount; c++)
                {
                    if (joint.connectedBody.transform.parent.GetChild(c) == joint.connectedBody.transform)
                    {
                        break;
                    }
                }

                connect.SetAttribute("Target", joint.connectedBody.transform.parent.gameObject.GetInstanceID().ToString()+":"+c);
            }
            else
            {
                connect.SetAttribute("Target", joint.connectedBody.gameObject.GetInstanceID().ToString());
            }

            connect.SetAttribute("MassScale", joint.connectedMassScale.ToString());
            connect.SetAttribute("Torque", joint.breakTorque.ToString());
            connect.SetAttribute("Force", joint.breakForce.ToString());
            transform.AppendChild(connect);
        }

        AttributeContainer[] attributeContainer = obj.GetComponents<AttributeContainer>();

        foreach (AttributeContainer ac in attributeContainer)
        {
            transform.AppendChild(ac.Serialize(doc));
        }

        foreach (Transform child in obj.transform)
        {
            Collider c = child.GetComponent<Collider>();
            if (c == null)
                continue;

            c.transform.parent = null;

            XmlElement @e = SerializeGameObject(c.gameObject, obj.transform, doc,shift,true,obj.gameObject);
            transform.AppendChild(e);

            c.transform.parent = obj.transform;
        }

        node.AppendChild(transform);
        return node;
    }

    public static XmlElement SerializeGameObject(GameObject obj,XmlDocument doc,string name)
    {
        XmlElement node;
        node = doc.CreateElement(name);
        XmlElement pos = doc.CreateElement("Position");
        Vector3 position = obj.transform.position;
        pos.SetAttribute("X", position.x.ToString());
        pos.SetAttribute("Y", position.y.ToString());
        pos.SetAttribute("Z", position.z.ToString());

        node.AppendChild(pos);

        XmlElement rotate = doc.CreateElement("Rotation");
        rotate.SetAttribute("X", obj.transform.rotation.x.ToString());
        rotate.SetAttribute("Y", obj.transform.rotation.y.ToString());
        rotate.SetAttribute("Z", obj.transform.rotation.z.ToString());
        rotate.SetAttribute("W", obj.transform.rotation.w.ToString());

        node.AppendChild(rotate);

        XmlElement scale = doc.CreateElement("Scale");
        scale.SetAttribute("X", obj.transform.localScale.x.ToString());
        scale.SetAttribute("Y", obj.transform.localScale.y.ToString());
        scale.SetAttribute("Z", obj.transform.localScale.z.ToString());

        node.AppendChild(scale);
        return node;
    }

    public static GameObject GenerateGameObject(string filename,Vector3 position,Quaternion rotate,bool ignore,int layer = 0)
    {
        XmlDocument content = new XmlDocument();
        content.Load(filename);

        XmlNodeList @list = content.GetElementsByTagName("Objects")[0].ChildNodes;
        XmlNode @center = (XmlElement)((XmlElement)content.GetElementsByTagName("Objects")[0]).GetElementsByTagName("CenterObject")[0];
        XmlElement @d_pos = ((XmlElement)((XmlElement)((XmlElement)center).GetElementsByTagName("Transform")[0]).GetElementsByTagName("Position")[0]);
        Vector3 @center_pos = new Vector3(float.Parse(d_pos.GetAttribute("X")), float.Parse(d_pos.GetAttribute("Y")), float.Parse(d_pos.GetAttribute("Z"))) + position;
        XmlElement @d_rot = ((XmlElement)((XmlElement)((XmlElement)center).GetElementsByTagName("Transform")[0]).GetElementsByTagName("Rotation")[0]);
        Quaternion prerot = new Quaternion(float.Parse(d_rot.GetAttribute("X")), float.Parse(d_rot.GetAttribute("Y")), float.Parse(d_rot.GetAttribute("Z")), float.Parse(d_rot.GetAttribute("W")));

        Dictionary<string, GameObject> map = new Dictionary<string, GameObject>();
        Dictionary<XmlElement, GameObject> map_obj = new Dictionary<XmlElement, GameObject>();
        GameObject @cent = null;
        for (int i = 0;i<list.Count;i++)
        {
            XmlElement @ele = (XmlElement)list[i];

            Crafting.dict.TryGetValue(int.Parse(ele.GetAttribute("id")), out GameObject origin);
            XmlElement @e_pos = (XmlElement)ele.GetElementsByTagName("Position")[0];
            Vector3 @pos = new Vector3(float.Parse(e_pos.GetAttribute("X")), float.Parse(e_pos.GetAttribute("Y")), float.Parse(e_pos.GetAttribute("Z"))); // Revive Position

            XmlElement @e_rot = (XmlElement)ele.GetElementsByTagName("Rotation")[0];
            Quaternion @rotation = new Quaternion(float.Parse(e_rot.GetAttribute("X")), float.Parse(e_rot.GetAttribute("Y")), float.Parse(e_rot.GetAttribute("Z")),float.Parse(e_rot.GetAttribute("W"))); // Revive Rotation

            GameObject @gobj = Instantiate(origin);
            gobj.GetComponent<Collider>().isTrigger = false;
            if(cent == null)
            {
                cent = gobj;
                if (ignore)
                {
                    gobj.transform.rotation = rotate;
                }
                else
                {
                    gobj.transform.rotation = rotate * rotation;
                }
                gobj.transform.position = center_pos;
            }
            else
            {
                gobj.transform.parent = cent.transform;
                gobj.transform.localRotation = rotation;
                gobj.transform.localPosition = pos;
                gobj.transform.parent = null;
            }

            //Memorize Rotation
            //gobj.GetComponent<ConnectiveMaterial>().Memorize();
            map.Add(ele.GetAttribute("uid"), gobj);
            map_obj.Add(ele, gobj);

            XmlNodeList @ol = ele.GetElementsByTagName("ChildObject");
            for (int c = 0; c < ol.Count; c++)
            {
                XmlElement @cele = (XmlElement)((XmlElement)ol[c]).GetElementsByTagName("Transform")[0];

                XmlElement @c_pos = (XmlElement)cele.GetElementsByTagName("Position")[0];
                Vector3 @cpos = new Vector3(float.Parse(c_pos.GetAttribute("X")), float.Parse(c_pos.GetAttribute("Y")), float.Parse(c_pos.GetAttribute("Z"))); // Revive Position

                XmlElement @c_rot = (XmlElement)cele.GetElementsByTagName("Rotation")[0];
                Quaternion @crotation = new Quaternion(float.Parse(c_rot.GetAttribute("X")), float.Parse(c_rot.GetAttribute("Y")), float.Parse(c_rot.GetAttribute("Z")), float.Parse(c_rot.GetAttribute("W"))); // Revive Rotation

                Transform child = gobj.transform.GetChild(c);

                AttributeContainer[] @container = child.GetComponents<AttributeContainer>();
                for (int a = 0; a < container.Length; a++)
                {
                    AttributeContainer attributeContainer = container[a];

                    attributeContainer.Deserialize((XmlElement)cele.GetElementsByTagName("Attributes")[a]);
                    attributeContainer.enabled = true;
                    attributeContainer.Apply();
                }

                HingeJoint hj = child.GetComponent<HingeJoint>();
                if (hj)
                {
                    JointSpring js = new JointSpring();
                    js.spring = Mathf.Abs(float.Parse(child.GetComponent<RotateMotion>().acceleration)) * 100;
                    js.targetPosition = float.Parse(cele.GetAttribute("CurrentPos"));
                    hj.spring = js;
                }
                else
                {
                    child.localPosition = cpos;
                    child.localRotation = crotation;
                }
            }

            XmlElement @e_sca = (XmlElement)ele.GetElementsByTagName("Scale")[0];
            Vector3 @scale = new Vector3(float.Parse(e_sca.GetAttribute("X")), float.Parse(e_sca.GetAttribute("Y")), float.Parse(e_sca.GetAttribute("Z"))); // Revive Scale

            gobj.transform.localScale = scale;

            try
            {
                gobj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                gobj.GetComponent<Rigidbody>().useGravity = true;
                gobj.GetComponent<Collider>().enabled = true;
                gobj.GetComponent<Rigidbody>().mass *= scale.x * scale.y * scale.z;
                /*foreach (AttributeContainer component in gobj.GetComponents<AttributeContainer>())
                {
                    component.enabled = true;
                }*/
            }
            catch { }
            gobj.layer = layer;

            AttributeContainer[] @ac = gobj.GetComponents<AttributeContainer>();
            for(int a = 0; a < ac.Length; a++)
            {
                ac[a].Deserialize((XmlElement)ele.GetElementsByTagName("Attributes")[a]);
                ac[a].Apply();
            }

            //Enable Function
            foreach (Transform child in gobj.transform)
            {
                child.gameObject.SetActive(true);

                try
                {
                    child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    child.GetComponent<Rigidbody>().useGravity = true;
                    child.GetComponent<Collider>().enabled = true;
                    child.GetComponent<Rigidbody>().mass *= scale.x * scale.y * scale.z;
                }
                catch
                {

                }
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            gobj.GetComponent<ConnectiveMaterial>().RefreshJoint();
        }

        foreach (XmlElement e in list)
        {
            XmlNodeList @l = e.GetElementsByTagName("Connection");
            if (l.Count > 0)
            {
                map_obj.TryGetValue(e, out GameObject obj);

                bool singleway = false;
                foreach (XmlElement ele in l)
                {
                    ConfigurableJoint j = obj.AddComponent<ConfigurableJoint>();

                    j.connectedMassScale = float.Parse(ele.GetAttribute("MassScale"));
                    //j.enableCollision = true;

                    //j.autoConfigureConnectedAnchor = false;

                    j.xMotion = ConfigurableJointMotion.Locked;
                    j.yMotion = ConfigurableJointMotion.Locked;
                    j.zMotion = ConfigurableJointMotion.Locked;
                    j.angularXMotion = ConfigurableJointMotion.Locked;
                    j.angularYMotion = ConfigurableJointMotion.Locked;
                    j.angularZMotion = ConfigurableJointMotion.Locked;

                    SoftJointLimit sj = new SoftJointLimit
                    {
                        contactDistance = float.PositiveInfinity
                    };
                    j.linearLimit = sj;
                    j.angularYLimit = sj;
                    j.angularZLimit = sj;
                    j.highAngularXLimit = sj;
                    j.lowAngularXLimit = sj;

                    j.projectionMode = JointProjectionMode.PositionAndRotation;

                    // j.enablePreprocessing = false;

                    GameObject @tar;

                    if (ele.GetAttribute("Target").Contains(":"))
                    {
                        try
                        {
                            string[] data = ele.GetAttribute("Target").Split(':');
                            map.TryGetValue(data[0], out GameObject p);
                            tar = p.transform.GetChild(int.Parse(data[1])).gameObject;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        map.TryGetValue(ele.GetAttribute("Target"), out tar);
                    }

                    if (!singleway)
                    {
                        if (tar.GetComponent<ConnectiveMaterial>() != null)
                        {
                            tar.GetComponent<ConnectiveMaterial>().sons.Add(obj);
                        }
                        else
                        {
                            tar.transform.parent.GetComponent<ConnectiveMaterial>().sons.Add(obj);
                        }
                        singleway = true;
                    }

                    j.breakForce = float.Parse(ele.GetAttribute("Force"));
                    j.breakTorque = float.Parse(ele.GetAttribute("Torque"));

                    Vector3 tmppos = tar.transform.position;
                    Quaternion tmprot = tar.transform.rotation;

                    tar.GetComponent<Rigidbody>().isKinematic = true;
                    try
                    {
                        XmlElement anchor = (XmlElement)ele.GetElementsByTagName("Anchor")[0];
                        tar.transform.position = obj.transform.TransformPoint(new Vector3(float.Parse(anchor.GetAttribute("X")), float.Parse(anchor.GetAttribute("Y")), float.Parse(anchor.GetAttribute("Z"))));
                        //j.connectedAnchor = new Vector3(float.Parse(anchor.GetAttribute("X")), float.Parse(anchor.GetAttribute("Y")), float.Parse(anchor.GetAttribute("Z")));
                        XmlElement anchorRotation = (XmlElement)ele.GetElementsByTagName("AnchorRotation")[0];
                        tar.transform.rotation = obj.transform.rotation * new Quaternion(float.Parse(anchorRotation.GetAttribute("X")), float.Parse(anchorRotation.GetAttribute("Y")), float.Parse(anchorRotation.GetAttribute("Z")), float.Parse(anchorRotation.GetAttribute("W")));
                    }
                    catch { }

                    try
                    {
                        j.connectedBody = tar.GetComponent<Rigidbody>();
                        j.targetRotation = Quaternion.Inverse(obj.transform.rotation) * tar.transform.rotation;
                        j.targetPosition = obj.transform.InverseTransformPoint(tar.transform.position);
                    }
                    catch (Exception er) { Debug.LogError(er); }
                    if (!ele.GetAttribute("Target").Contains(":"))
                    {
                        tar.transform.position = tmppos;
                        tar.transform.rotation = tmprot;
                    }
                    tar.GetComponent<Rigidbody>().isKinematic = false;

                }
            }

            XmlNodeList childs = e.GetElementsByTagName("ChildObject");
            for (int i = 0 ; i < childs.Count;i++)
            {
                XmlElement c = (XmlElement)childs[i];
                l = c.GetElementsByTagName("ChildConnection");
                if (l.Count > 0)
                {
                    map_obj.TryGetValue(e, out GameObject parent);
                    GameObject obj = parent.transform.GetChild(i).gameObject;

                    bool singleway = false;
                    foreach (XmlElement ele in l)
                    {
                        ConfigurableJoint j = obj.AddComponent<ConfigurableJoint>();

                        j.connectedMassScale = float.Parse(ele.GetAttribute("MassScale"));
                        //j.enableCollision = true;

                        //j.autoConfigureConnectedAnchor = false;

                        j.xMotion = ConfigurableJointMotion.Locked;
                        j.yMotion = ConfigurableJointMotion.Locked;
                        j.zMotion = ConfigurableJointMotion.Locked;
                        j.angularXMotion = ConfigurableJointMotion.Locked;
                        j.angularYMotion = ConfigurableJointMotion.Locked;
                        j.angularZMotion = ConfigurableJointMotion.Locked;

                        SoftJointLimit sj = new SoftJointLimit
                        {
                            contactDistance = float.PositiveInfinity
                        };
                        j.linearLimit = sj;
                        j.angularYLimit = sj;
                        j.angularZLimit = sj;
                        j.highAngularXLimit = sj;
                        j.lowAngularXLimit = sj;

                        j.projectionMode = JointProjectionMode.PositionAndRotation;

                       // j.enablePreprocessing = false;

                        GameObject @tar;

                        if (ele.GetAttribute("Target").Contains(":"))
                        {
                            string[] data = ele.GetAttribute("Target").Split(':');
                            map.TryGetValue(data[0], out GameObject p);
                            tar = p.transform.GetChild(int.Parse(data[1])).gameObject;
                        }
                        else
                        {
                            map.TryGetValue(ele.GetAttribute("Target"), out tar);
                        }

                        if (!singleway)
                        {
                            if (tar.GetComponent<ConnectiveMaterial>() != null)
                            {
                                tar.GetComponent<ConnectiveMaterial>().sons.Add(obj);
                            }
                            else
                            {
                                tar.transform.parent.GetComponent<ConnectiveMaterial>().sons.Add(obj);
                            }
                            singleway = true;
                        }

                        j.breakForce = float.Parse(ele.GetAttribute("Force"));
                        j.breakTorque = float.Parse(ele.GetAttribute("Torque"));

                        Vector3 tmppos = tar.transform.position;
                        Quaternion tmprot = tar.transform.rotation;

                        tar.GetComponent<Rigidbody>().isKinematic = true;
                        try
                        {
                            XmlElement anchor = (XmlElement)ele.GetElementsByTagName("Anchor")[0];
                            tar.transform.position = obj.transform.TransformPoint(new Vector3(float.Parse(anchor.GetAttribute("X")), float.Parse(anchor.GetAttribute("Y")), float.Parse(anchor.GetAttribute("Z"))));
                            XmlElement anchorRotation = (XmlElement)ele.GetElementsByTagName("AnchorRotation")[0];
                            tar.transform.rotation = obj.transform.rotation * new Quaternion(float.Parse(anchorRotation.GetAttribute("X")), float.Parse(anchorRotation.GetAttribute("Y")), float.Parse(anchorRotation.GetAttribute("Z")), float.Parse(anchorRotation.GetAttribute("W")));
                        }
                        catch { }

                        try
                        {
                            j.connectedBody = tar.GetComponent<Rigidbody>();
                            j.targetRotation = Quaternion.Inverse(obj.transform.rotation) * tar.transform.rotation;
                            j.targetPosition = obj.transform.InverseTransformPoint(tar.transform.position);
                        }
                        catch { }
                        tar.GetComponent<Rigidbody>().isKinematic = false;

                        tar.transform.position = tmppos;
                        tar.transform.rotation = tmprot;
                        
                    }
                }
            }
        }

        CenterObject cto = cent.AddComponent<CenterObject>();
        cto.Regist();
        cto.id = int.Parse(((XmlElement)center).GetAttribute("ID"));
        cto.prerot = prerot;
        foreach (GameObject sons in map_obj.Values)
        {
            cto.objlist.Add(sons);
            if(sons != cent)
            {
                sons.GetComponent<ConnectiveMaterial>().centerparent = cent;
                foreach(Transform child in sons.transform)
                {
                    if(child.GetComponent<ConnectiveMaterial>() != null)
                        child.GetComponent<ConnectiveMaterial>().centerparent = cent;
                }
            }
        }
        return cent;
    }

    public static void SaveCharacter(GameObject maincharacter,GameObject camera,string filename){
        if (!Directory.Exists(filename.Substring(0,filename.LastIndexOf("/"))))
        {
            Directory.CreateDirectory(filename.Substring(0, filename.LastIndexOf("/")));
        }

        XmlDocument content = new XmlDocument();
        XmlDeclaration dec = content.CreateXmlDeclaration("1.0", "UTF-8", null);
        content.AppendChild(dec);

        XmlElement objs = content.CreateElement("Character");

        XmlElement node = SerializeGameObject(maincharacter, content,"Position");
        objs.AppendChild(node);
        node = SerializeGameObject(camera, content,"Camera");
        objs.AppendChild(node);

        node = content.CreateElement("Hotbar");
        foreach (GameObject item in Inventory.m.hotbar)
        {
            ItemHolder ih = item.GetComponent<ItemHolder>();
            node.AppendChild(ih.AddXml(content));
        }
        objs.AppendChild(node);

        node = content.CreateElement("Items");

        foreach(GameObject item in Inventory.holders)
        {
            ItemHolder ih = item.GetComponent<ItemHolder>();
            node.AppendChild(ih.AddXml(content));
        }

        objs.AppendChild(node);

        PlayerEntity pe = maincharacter.GetComponent<PlayerEntity>();
        objs.SetAttribute("Health", "" + pe.nowHealth);
        objs.SetAttribute("Thrist", "" + pe.Thrist);
        objs.SetAttribute("Satisfaction", "" + pe.Satisfaction);

        content.AppendChild(objs);

        Debug.Log("Saved to:"+filename);
        content.Save(filename);
    }

    public static void SaveWorld(string filename)
    {
        if (!Directory.Exists(filename.Substring(0, filename.LastIndexOf("/"))))
        {
            Directory.CreateDirectory(filename.Substring(0, filename.LastIndexOf("/")));
        }

        XmlDocument content = new XmlDocument();
        XmlDeclaration dec = content.CreateXmlDeclaration("1.0", "UTF-8", null);
        content.AppendChild(dec);

        XmlElement objs = content.CreateElement("World");

        objs.SetAttribute("RandomSeed", "" + GameControll.randomseed);
        objs.SetAttribute("WorldTime", "" + DayNight.rot);
        objs.SetAttribute("RainTime", "" + DayNight.dn.startTime);
        objs.SetAttribute("PlayTime", "" + TimeManager.GetCurrentPlayTime());
        objs.SetAttribute("Duration", "" + DayNight.dn.duration);

        XmlElement nt = content.CreateElement("CustomItems");

        XmlElement node;
        foreach (int id in Inventory.allocateItem)
        {
            node = content.CreateElement("Item");
            node.SetAttribute("id", id + "");
            nt.AppendChild(node);
        }

        objs.AppendChild(nt);
        content.AppendChild(objs);

        Debug.Log("Saved to:" + filename);
        content.Save(filename);
    }

    public static void ReadWorld(string filename)
    {
        XmlDocument content = new XmlDocument();
        content.Load(filename);

        XmlElement @list = (XmlElement)content.GetElementsByTagName("World")[0];

        GameControll.globalRandomize = new System.Random(int.Parse(list.GetAttribute("RandomSeed")));
        GameControll.randomseed = int.Parse(list.GetAttribute("RandomSeed"));
        DayNight.rot = float.Parse(list.GetAttribute("WorldTime"));
        DayNight.dn.startTime = float.Parse(list.GetAttribute("RainTime"));
        TimeManager.SetLastPlayTime(float.Parse(list.GetAttribute("PlayTime")));
        DayNight.dn.duration = int.Parse(list.GetAttribute("Duration"));

        try
        {
            XmlNodeList @nl = list.GetElementsByTagName("CustomItems")[0].ChildNodes;

            foreach (XmlElement node in nl)
            {
                Inventory.allocateItem.Add(int.Parse(node.GetAttribute("id")));
            }
        }
        catch
        {
        }
    }

    public static void ReadCharacter(GameObject mc,GameObject cam,string filename)
    {
        XmlDocument content = new XmlDocument();
        content.Load(filename);

        XmlElement @list = (XmlElement)content.GetElementsByTagName("Character")[0];

        PlayerEntity pe = mc.GetComponent<PlayerEntity>();
        pe.nowHealth = float.Parse(list.GetAttribute("Health"));
        pe.Thrist = float.Parse(list.GetAttribute("Thrist"));
        pe.Satisfaction = float.Parse(list.GetAttribute("Satisfaction"));

        XmlElement @pos = (XmlElement)list.GetElementsByTagName("Position")[0];

        XmlElement @e_pos = (XmlElement)pos.GetElementsByTagName("Position")[0];
        mc.transform.position = new Vector3(float.Parse(e_pos.GetAttribute("X")), float.Parse(e_pos.GetAttribute("Y")), float.Parse(e_pos.GetAttribute("Z")));

        XmlElement @e_rot = (XmlElement)pos.GetElementsByTagName("Rotation")[0];
        mc.transform.rotation = new Quaternion(float.Parse(e_rot.GetAttribute("X")), float.Parse(e_rot.GetAttribute("Y")), float.Parse(e_rot.GetAttribute("Z")),float.Parse(e_rot.GetAttribute("W")));

        XmlElement @r_cam = (XmlElement)((XmlElement)list).GetElementsByTagName("Camera")[0];

        @e_rot = (XmlElement)r_cam.GetElementsByTagName("Rotation")[0];
        cam.transform.rotation = new Quaternion(float.Parse(e_rot.GetAttribute("X")), float.Parse(e_rot.GetAttribute("Y")), float.Parse(e_rot.GetAttribute("Z")),float.Parse(e_rot.GetAttribute("W")));

        Inventory.m.Reload(list.GetElementsByTagName("Items")[0].ChildNodes, list.GetElementsByTagName("Hotbar")[0].ChildNodes);
    }

    public struct Values
    {
        public object values;
        public string filename;

        public Values(object vs, string fn)
        {
            values = vs;
            filename = fn;
        }
    }

    public static void SaveData(object values)
    {
        Values val = (Values)values;

        if (!Directory.Exists(val.filename.Substring(0, val.filename.LastIndexOf("/"))))
        {
            Directory.CreateDirectory(val.filename.Substring(0, val.filename.LastIndexOf("/")));
        }
        bool block = true;
        while (block)
        {
            try
            {

                using (FileStream stream = new FileStream(val.filename, FileMode.OpenOrCreate))
                {
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        using (var zipStream = new GZipStream(stream, CompressionMode.Compress))
                        {
                            try
                            {
                                formatter.Serialize(zipStream, val.values);
                                block = false;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
            catch
            {

            }
            Thread.Sleep(5);
        }
    }

    public static TResult DeserializeFromFile<TResult>(string filename) where TResult : class
    {
        using (FileStream stream = new FileStream(filename, FileMode.Open))
        {
            //MemoryStream ms = new MemoryStream();
            using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                TResult result = formatter.Deserialize(zipStream) as TResult;

                return result;
            }
        }
    }

}
